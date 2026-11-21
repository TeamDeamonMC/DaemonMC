using System.IO.Compression;
using System.Numerics;
using DaemonMC.Biomes;
using DaemonMC.Blocks;
using DaemonMC.Entities;
using DaemonMC.Level.Format;
using DaemonMC.Level.Generators;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;
using fNbt;
using MiNET.LevelDB;
using Newtonsoft.Json;

namespace DaemonMC.Level
{
    public class World
    {
        internal static string[] LevelDbVersion { get; } = ["1.26.60"];
        public Dictionary<(int x, int z), Chunk> Cache { get; set; } = new Dictionary<(int x, int z), Chunk>();
        public SuperFlat Generator { get; set; } = new SuperFlat();
        public string LevelName { get; set; } = "";
        public Dictionary<long, Player> OnlinePlayers { get; set; } = new Dictionary<long, Player>();
        public Dictionary<long, Entity> Entities { get; set; } = new Dictionary<long, Entity>();
        public Dictionary<string, GameRule> GameRules { get; set; } = new Dictionary<string, GameRule>();
        public Database Db { get; set; }
        public string LevelDisplayName { get; set; } = "";
        public int Version { get; set; } = Info.ProtocolVersion.Last();
        public int SpawnX { get; set; } = 0;
        public int SpawnY { get; set; } = 384;
        public int SpawnZ { get; set; } = 0;
        public long RandomSeed { get; set; } = 0;
        public Dimension Dimension { get; } = Dimension.Overworld;
        public World Overworld { get; }
        public int MinY => Dimension.MinY();
        public int MaxY => Dimension.MaxY();
        private HashSet<(int x, int z)> EntityChunksLoaded { get; } = new HashSet<(int x, int z)>();
        private readonly Dictionary<Dimension, World> dimensions = new Dictionary<Dimension, World>();
        private readonly object dimensionLock = new object();
        private static readonly byte[] DigestPrefix = "digp"u8.ToArray();
        private static readonly byte[] ActorPrefix = "actorprefix"u8.ToArray();

        public World(string levelName)
        {
            LevelName = levelName;
            Overworld = this;
            dimensions[Dimension.Overworld] = this;
            Load();
            UnloadChunks();
        }

        private World(World overworld, Dimension dimension)
        {
            Overworld = overworld;
            Dimension = dimension;
            LevelName = overworld.LevelName;
            LevelDisplayName = overworld.LevelDisplayName;
            Version = overworld.Version;
            RandomSeed = overworld.RandomSeed;
            GameRules = overworld.GameRules;
            Db = overworld.Db;
            SpawnX = overworld.SpawnX;
            SpawnZ = overworld.SpawnZ;
            Generator = dimension switch
            {
                Dimension.Nether => new SuperFlat { Layers = [new Bedrock(), new Netherrack(), new Netherrack(), new Netherrack()] },
                Dimension.End => new SuperFlat { Layers = [new EndStone(), new EndStone(), new EndStone()] },
                _ => new SuperFlat()
            };
            CalculateSpawnY();
            UnloadChunks();
        }

        public World GetDimension(Dimension dimension)
        {
            var root = Overworld;
            lock (root.dimensionLock)
            {
                if (!root.dimensions.TryGetValue(dimension, out var world))
                {
                    Log.debug($"Loading dimension {dimension} of world: {LevelName}");
                    world = new World(root, dimension);
                    root.dimensions[dimension] = world;
                }
                return world;
            }
        }

        public IReadOnlyCollection<World> LoadedDimensions
        {
            get
            {
                lock (Overworld.dimensionLock)
                {
                    return Overworld.dimensions.Values.ToList();
                }
            }
        }

        private byte[] ChunkKey(int x, int z)
        {
            byte[] key = ToDataTypes.GetByteSum(BitConverter.GetBytes(x), BitConverter.GetBytes(z));
            if (Dimension != Dimension.Overworld)
            {
                key = ToDataTypes.GetByteSum(key, BitConverter.GetBytes((int)Dimension));
            }
            return key;
        }

        public void Send(Packet packet, long selfId = -1)
        {
            foreach (var dest in OnlinePlayers)
            {
                if (dest.Value.EntityID == selfId)
                {
                    continue;
                }
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                packet.EncodePacket(encoder);
            }
        }

        public void SendLevelEvent(Vector3 pos, LevelEvents value, int data = 0)
        {
            var packet = new LevelEvent
            {
                EventID = value,
                Position = pos,
                Data = data
            };
            Send(packet);
        }

        public void SetBlock(Block block, Vector3 playerPos, bool sendBlock = true)
        {
            SetBlock(block, (int)(playerPos.X < 0 ? playerPos.X - 1 : playerPos.X), (int)playerPos.Y - 3, (int)(playerPos.Z < 0 ? playerPos.Z - 1 : playerPos.Z));
        }

        public void SetBlock(Block block, int x, int y, int z, bool sendBlock = true)
        {
            if (sendBlock)
            {
                SendBlock(block, x, y, z);
            }

            int chunkX = (int)Math.Floor(x / 16f);
            int chunkZ = (int)Math.Floor(z / 16f);

            if (Cache.TryGetValue((chunkX, chunkZ), out Chunk chunk))
            {
                int subChunkY = (int)Math.Floor((y - MinY) / 16f);

                if (subChunkY < 0 || subChunkY >= Dimension.SubChunkCount())
                {
                    Log.warn($"Tried to set block outside of world height. In '{LevelName}' ({Dimension}) position x:{x}; y:{y}, z:{z}");
                    return;
                }

                int localX = ((int)x % 16 + 16) % 16;
                int localY = ((int)y % 16 + 16) % 16;
                int localZ = ((int)z % 16 + 16) % 16;

                if (!BlockPalette.blockHashes.ContainsKey(block.GetHash()))
                {
                    Log.warn($"Blockstate hash for {block.Name} not found in BlockPalette. Requested state: {JsonConvert.SerializeObject(block.States, Formatting.Indented)}");
                    return;
                }

                while (chunk.Chunks.Count <= subChunkY)
                {
                    var emptySubChunk = ChunkUtils.CreateEmptySubChunk();
                    emptySubChunk.BiomePalette.Add(BiomeManager.GetBiomeId("plains"));
                    chunk.Chunks.Add(emptySubChunk);
                }

                var subChunk = chunk.Chunks[subChunkY];

                if (subChunk.StorageSize == 0 || subChunk.Palette.Count == 0)
                {
                    var emptySubChunk = ChunkUtils.CreateEmptySubChunk();
                    emptySubChunk.BiomePalette = subChunk.BiomePalette;
                    chunk.Chunks[subChunkY] = emptySubChunk;
                    subChunk = emptySubChunk;
                }

                if (!NbtUtils.Contains(subChunk.Palette, block.GetState()))
                {
                    subChunk.Palette.Add(block.GetState());
                    subChunk.BitsPerBlock = Math.Max(subChunk.BitsPerBlock, ChunkUtils.GetBitsPerBlock(subChunk.Palette.Count));
                }

                int blockIndex = (localY) | (localZ << 4) | (localX << 8);
                byte stateIndex = (byte)NbtUtils.IndexOf(subChunk.Palette, block.GetState());
                subChunk.Blocks[blockIndex] = stateIndex;

                chunk.RemoveBlockEntity(x, y, z);
            }
            else
            {
                Log.warn($"Tried to access unloaded chunk x:{chunkX}; z:{chunkZ}. In '{LevelName}' position x:{x}; y:{y}, z:{z}");
            }
        }

        public NbtCompound? GetBlockEntity(int x, int y, int z)
        {
            if (Cache.TryGetValue(((int)Math.Floor(x / 16f), (int)Math.Floor(z / 16f)), out Chunk chunk))
            {
                return chunk.GetBlockEntity(x, y, z);
            }
            return null;
        }

        public bool SetBlockEntity(NbtCompound blockEntity, bool send = true)
        {
            if (!Chunk.TryGetPosition(blockEntity, out int x, out int y, out int z))
            {
                Log.warn($"Tried to set block entity without x, y, z position in '{LevelName}'");
                return false;
            }

            if (!Cache.TryGetValue(((int)Math.Floor(x / 16f), (int)Math.Floor(z / 16f)), out Chunk chunk))
            {
                Log.warn($"Tried to set block entity in unloaded chunk. In '{LevelName}' position x:{x}; y:{y}, z:{z}");
                return false;
            }

            chunk.SetBlockEntity(blockEntity);

            if (send)
            {
                Send(new BlockActorData
                {
                    Position = new Vector3(x, y, z),
                    Nbt = blockEntity
                });
            }
            return true;
        }

        public bool RemoveBlockEntity(int x, int y, int z)
        {
            if (Cache.TryGetValue(((int)Math.Floor(x / 16f), (int)Math.Floor(z / 16f)), out Chunk chunk))
            {
                return chunk.RemoveBlockEntity(x, y, z);
            }
            return false;
        }

        public void SendBlock(Block block, int x, int y, int z)
        {
            var packet = new UpdateBlock()
            {
                Block = block,
                Position = new Vector3(x, y, z)
            };
            Send(packet);
        }

        public void SendBlock(Block block, Vector3 playerPos)
        {
            SendBlock(block, (int)(playerPos.X < 0 ? playerPos.X - 1 : playerPos.X), (int)playerPos.Y - 3, (int)(playerPos.Z < 0 ? playerPos.Z - 1 : playerPos.Z));
        }

        public void SendSound(LevelSoundEvent sound, Vector3 playerPos)
        {
            var packet = new LevelSoundEventPacket()
            {
                EventID = (int)sound,
                Position = playerPos
            };
            Send(packet);
        }

        public void SendMessage(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 0,
                Message = message
            };
            Send(packet);
        }

        public void SendChatMessage(string message, string from)
        {
            var packet = new TextMessage
            {
                MessageType = 1,
                Message = message,
                Username = from
            };
            Send(packet);
        }

        public void SendPopup(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 3,
                Message = message
            };
            Send(packet);
        }

        public void SendJukeboxPopup(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 4,
                Message = message
            };
            Send(packet);
        }

        public void SendTip(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 5,
                Message = message
            };
            Send(packet);
        }

        public void SendTitle(string title, string subtitle = "", int fadeInTime = 1, int stayTime = 1, int fadeOutTime = 1)
        {
            foreach (var dest in OnlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest);
                var packet = new SetTitle
                {
                    Type = 2,
                    Text = title,
                    FadeIn = fadeInTime * 20,
                    Stay = stayTime * 20,
                    FadeOut = fadeOutTime * 20,
                    XUID = dest.XUID
                };
                packet.EncodePacket(encoder);

                if (subtitle != "")
                {
                    var packet2 = new SetTitle
                    {
                        Type = 3,
                        Text = subtitle,
                        FadeIn = fadeInTime * 20,
                        Stay = stayTime * 20,
                        FadeOut = fadeOutTime * 20,
                        XUID = dest.XUID
                    };
                }
            }
        }

        public void SendActionBarTitle(string title, int fadeInTime = 1, int stayTime = 1, int fadeOutTime = 1)
        {
            foreach (var dest in OnlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest);
                var packet = new SetTitle
                {
                    Type = 4,
                    Text = title,
                    FadeIn = fadeInTime * 20,
                    Stay = stayTime * 20,
                    FadeOut = fadeOutTime * 20,
                    XUID = dest.XUID
                };
                packet.EncodePacket(encoder);
            }
        }

        public void ClearTitle()
        {
            foreach (var dest in OnlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest);
                var packet = new SetTitle
                {
                    Type = 0,
                    XUID = dest.XUID
                };
                packet.EncodePacket(encoder);
            }
        }

        public void SendToast(string title, string content)
        {
            var packet = new ToastRequest();
            packet.Title = title;
            packet.Body = content;
            Send(packet);
        }

        public void SendTime(int time)
        {
            var packet = new SetTime()
            {
                Time = time,
            };
            Send(packet);
        }

        public void AddPlayer(Player player)
        {
            OnlinePlayers.TryAdd(player.EntityID, player);
            foreach (var entity in Entities.Values)
            {
                if (entity is CustomEntity customEntity)
                {
                    var packet = new AddPlayer
                    {
                        UUID = customEntity.UUID,
                        Username = customEntity.NameTag,
                        EntityId = customEntity.EntityId,
                        Position = customEntity.Position,
                        Metadata = customEntity.Metadata,
                    };
                    player.Send(packet);

                    var packet2 = new PlayerList
                    {
                        UUID = customEntity.UUID,
                        EntityId = customEntity.EntityId,
                        Username = customEntity.NameTag,
                        Skin = customEntity.Skin
                    };
                    player.Send(packet2);
                }
                else
                {
                    var pk = new AddActor
                    {
                        EntityId = entity.EntityId,
                        ActorType = entity.ActorType,
                        Position = entity.Position,
                        Velocity = entity.Velocity,
                        Rotation = entity.Rotation,
                        YheadRotation = entity.YheadRotation,
                        YbodyRotation = entity.YbodyRotation,
                        Attributes = entity.Attributes,
                        Metadata = entity.Metadata,
                        Properties = entity.Properties
                    };
                    player.Send(pk);
                }
            }

            var packet3 = new PlayerList
            {
                UUID = player.UUID,
                EntityId = player.EntityID,
                Username = player.Username,
                XUID = player.XUID,
                Skin = player.Skin
            };
            Send(packet3);

            foreach (Player onlinePlayer in OnlinePlayers.Values)
            {
                var packet4 = new PlayerList
                {
                    UUID = onlinePlayer.UUID,
                    EntityId = onlinePlayer.EntityID,
                    Username = onlinePlayer.Username,
                    XUID = onlinePlayer.XUID,
                    Skin = onlinePlayer.Skin
                };
                Send(packet4);

                if (onlinePlayer == player) { continue; }

                var packet = new AddPlayer
                {
                    UUID = player.UUID,
                    Username = player.Username,
                    EntityId = player.EntityID,
                    Position = player.Position,
                    Metadata = player.Metadata,
                    Layers = player.Abilities,
                    Item = player.Inventory.GetHand()
                };
                onlinePlayer.Send(packet);

                var armor = new MobArmorEquipment
                {
                    EntityId = player.EntityID,
                    Head = player.Inventory.Head,
                    Chest = player.Inventory.Chest,
                    Legs = player.Inventory.Legs,
                    Feet = player.Inventory.Feets
                };
                onlinePlayer.Send(armor);

                var packet2 = new AddPlayer
                {
                    UUID = onlinePlayer.UUID,
                    Username = onlinePlayer.Username,
                    EntityId = onlinePlayer.EntityID,
                    Position = onlinePlayer.Position,
                    Metadata = onlinePlayer.Metadata,
                    Layers = onlinePlayer.Abilities,
                    Item = onlinePlayer.Inventory.GetHand()
                };
                player.Send(packet2);

                var armor2 = new MobArmorEquipment
                {
                    EntityId = onlinePlayer.EntityID,
                    Head = onlinePlayer.Inventory.Head,
                    Chest = onlinePlayer.Inventory.Chest,
                    Legs = onlinePlayer.Inventory.Legs,
                    Feet = onlinePlayer.Inventory.Feets
                };
                player.Send(armor2);
            }
            Log.info($"{player.Username} spawned in World:'{LevelName}' ({Dimension}) X:{player.Position.X} Y:{player.Position.Y} Z:{player.Position.Z}");
        }

        public void RemovePlayer(Player player)
        {
            if (!OnlinePlayers.Remove(player.EntityID))
            {
                Log.warn($"Couldn't despawn {player.Username} from World {player.CurrentWorld.LevelName}.");
            }
            else
            {
                var packet = new RemoveActor
                {
                    EntityId = player.EntityID
                };
                Send(packet);

                var packet1 = new PlayerList
                {
                    Action = 1,
                    UUID = player.UUID,
                };
                Send(packet1);

                Log.debug($"Despawned {player.Username} from World {player.CurrentWorld.LevelName}");
            }
        }

        public Chunk GetChunk(int x, int z)
        {
            if (Cache.TryGetValue((x, z), out var cachedChunk))
            {
                return cachedChunk;
            }

            byte[] dataKey = ToDataTypes.GetByteSum(ChunkKey(x, z), new byte[] { 0x2f, 0 });

            var chunk = new Chunk();

            int subChunkCount = Dimension.SubChunkCount();
            int minSubChunkIndex = MinY >> 4;
            List<byte[]> subChunks = new List<byte[]>(subChunkCount);
            int lastSubChunkY = -1;

            for (int y = 0; y < subChunkCount; y++)
            {
                dataKey[^1] = (byte)(y + minSubChunkIndex);
                byte[] subChunk = Db.Get(dataKey);

                subChunks.Add(subChunk);
                if (subChunk != null)
                {
                    lastSubChunkY = y;
                }
            }

            if (lastSubChunkY == -1)
            {
                chunk = Generator.GenerateChunk();
            }

            for (int y = 0; y <= lastSubChunkY; y++)
            {
                byte[] subChunk = subChunks[y];

                if (subChunk != null)
                {
                    try
                    {
                        chunk.Chunks.Add(ChunkUtils.DecodeSubChunk(subChunk));
                    }
                    catch (Exception ex)
                    {
                        Log.error($"SubChunk x:{x}; y:{y} z:{z} decoding failed. This shouldn't happen. Sent empty chunk");
                        Log.error(ex.ToString());
                        chunk.Chunks.Add(new SubChunk());
                    }
                }
                else
                {
                    chunk.Chunks.Add(new SubChunk());
                }
            }

            for (int i = 0; i < chunk.Chunks.Count; i++) //todo temp code to apply plains biome. Need to read these from DB
            {
                chunk.Chunks[i].BiomePalette.Add(BiomeManager.GetBiomeId("plains"));
                //chunk.Chunks[i].Biomes = Enumerable.Repeat((byte)0x01, 4096).ToArray();
            }

            byte[] blockEntityData = Db.Get(ToDataTypes.GetByteSum(ChunkKey(x, z), new byte[] { 0x31 }));
            if (blockEntityData != null)
            {
                try
                {
                    chunk.LoadBlockEntities(blockEntityData);
                }
                catch (Exception ex)
                {
                    Log.error($"Block entities in chunk x:{x}; z:{z} in '{LevelName}' could not be loaded");
                    Log.error(ex.ToString());
                }
            }

            Cache.TryAdd((x, z), chunk);

            if (EntityChunksLoaded.Add((x, z)))
            {
                LoadEntities(x, z);
            }

            return chunk;
        }

        public List<Entity> LoadEntities(int x, int z)
        {
            var loaded = new List<Entity>();

            byte[] digestKey = ToDataTypes.GetByteSum(DigestPrefix, ChunkKey(x, z));
            byte[] digest = Db.Get(digestKey);

            if (digest == null || digest.Length < 8)
            {
                return loaded;
            }

            for (int i = 0; i + 8 <= digest.Length; i += 8)
            {
                byte[] actorKey = ToDataTypes.GetByteSum(ActorPrefix, digest.AsSpan(i, 8).ToArray());
                byte[] actorData = Db.Get(actorKey);

                if (actorData == null)
                {
                    Log.warn($"Entity record {Convert.ToHexString(actorKey, ActorPrefix.Length, 8)} listed in chunk x:{x} z:{z} not found in '{LevelName}'");
                    continue;
                }

                try
                {
                    var nbt = new NbtFile
                    {
                        BigEndian = false,
                        UseVarInt = false
                    };
                    nbt.LoadFromBuffer(actorData, 0, actorData.Length, NbtCompression.None);
                    var tag = (NbtCompound)nbt.RootTag;

                    string identifier = tag.Get<NbtString>("identifier")?.Value ?? "";
                    if (identifier == "")
                    {
                        Log.debug($"Skipped loading empty entity in chunk x:{x} z:{z} of '{LevelName}'");
                        continue;
                    }

                    Entity entity = EntityPalette.Create(identifier);
                    entity.LoadNbt(tag);
                    entity.Spawn(this);
                    loaded.Add(entity);
                }
                catch (Exception ex)
                {
                    Log.error($"Failed to load entity in chunk x:{x} z:{z} of '{LevelName}': {ex.Message}");
                }
            }

            if (loaded.Count > 0)
            {
                Log.debug($"Loaded {loaded.Count} entities from chunk x:{x} z:{z} in '{LevelName}'");
            }

            return loaded;
        }

        public void Load()
        {
            var tempData = Path.Combine(Path.GetTempPath(), $"{LevelName}.mcworld");
            if (Directory.Exists(tempData))
            {
                Directory.Delete(tempData, true);
            }
            if (!File.Exists($"Worlds/{LevelName}.mcworld"))
            {
                Log.warn($"World Worlds/{LevelName}.mcworld not found. Creating new flat world...");
                try
                {
                    WorldCreator.Create($"Worlds/{LevelName}.mcworld", LevelName, Generator);
                }
                catch (Exception ex)
                {
                    Log.error($"Failed to create world Worlds/{LevelName}.mcworld: {ex.Message}");
                    Server.Crash = true;
                    return;
                }
            }

            Log.info($"Loading world: Worlds\\{LevelName}.mcworld");

            string worldPath = $"Worlds/{LevelName}.mcworld";
            NbtCompound? levelDat = ReadLevelDat(worldPath);
            if (levelDat == null)
            {
                Log.error($"level.dat not found in {worldPath}");
                Server.Crash = true;
                return;
            }

            System.Version? worldVersion = WorldUpgrader.GetWorldVersion(levelDat);
            int worldProtocol;
            if (worldVersion != null)
            {
                if (worldVersion > System.Version.Parse(LevelDbVersion.Last()))
                {
                    Log.error($"World {worldPath} was saved with Minecraft {worldVersion}, which is newer than this server supports ({LevelDbVersion.Last()}). Downgrading worlds is not supported.");
                    Server.Crash = true;
                    return;
                }
                worldProtocol = WorldUpgrader.ToProtocol(worldVersion);
            }
            else
            {
                worldProtocol = levelDat.TryGet<NbtInt>("NetworkVersion", out var networkVersion) ? networkVersion!.Value : 0;
            }

            if (StateConverter.NeedsUpgrade(worldProtocol))
            {
                if (!WorldUpgrader.Upgrade(worldPath, worldProtocol, worldVersion?.ToString() ?? $"protocol {worldProtocol}"))
                {
                    Server.Crash = true;
                    return;
                }
                levelDat = ReadLevelDat(worldPath)!;
            }

            RandomSeed = levelDat["RandomSeed"].LongValue;
            LevelDisplayName = levelDat["LevelName"].StringValue;
            Version = levelDat["NetworkVersion"].IntValue;
            SpawnX = levelDat["SpawnX"].IntValue;
            SpawnZ = levelDat["SpawnZ"].IntValue;
            GameRules.Add("showCoordinates", new GameRule(true));

            Db = new Database(new DirectoryInfo(worldPath));
            Db.Open();

            CalculateSpawnY();
        }

        private static NbtCompound? ReadLevelDat(string mcworldPath)
        {
            using ZipArchive archive = ZipFile.OpenRead(mcworldPath);
            var entry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith("level.dat", StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                return null;
            }

            using Stream stream = entry.Open();
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 8; // storage version + payload length

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false
            };
            nbt.LoadFromStream(memoryStream, NbtCompression.None);
            return (NbtCompound)nbt.RootTag;
        }

        private void CalculateSpawnY()
        {
            var SpawnChunk = GetChunk(SpawnX >> 4, SpawnZ >> 4);
            if (SpawnChunk.Chunks.Count == 0)
            {
                SpawnY = MaxY;
                Log.warn($"Unable to calulate spawn point Y value for {LevelName} ({Dimension}). Spawn chunk not found in DB. Y value set to {SpawnY}");
            }
            else
            {
                SpawnY = Math.Min(MinY + SpawnChunk.Chunks.Count * 16, MaxY);
            }
            Log.debug($"Calculated '{LevelName}' ({Dimension}) spawn point: X:{SpawnX} Y:{SpawnY} Z:{SpawnZ}");
        }

        public void UnloadChunks()
        {
            if (!DaemonMC.UnloadChunks)
            {
                return;
            }
            _ = Task.Run(async () =>
            {
                if (Cache.Count > 0)
                {
                    var activeChunks = new HashSet<(int x, int z)>();

                    foreach (var player in OnlinePlayers.Values)
                    {
                        int playerChunkX = (int)Math.Floor(player.Position.X / 16.0);
                        int playerChunkZ = (int)Math.Floor(player.Position.Z / 16.0);
                        int distance = player.drawDistance / 2;

                        for (int dx = -distance; dx <= distance; dx++)
                        {
                            for (int dz = -distance; dz <= distance; dz++)
                            {
                                activeChunks.Add((playerChunkX + dx, playerChunkZ + dz));
                            }
                        }
                    }

                    var chunksToRemove = Cache.Keys
                        .Where(chunkCoord => !activeChunks.Contains(chunkCoord) && chunkCoord != (SpawnX >> 4, SpawnZ >> 4))
                        .ToList();

                    int loadedChunks = Cache.Count();

                    if (chunksToRemove.Count != 0)
                    {
                        foreach (var chunkCoord in chunksToRemove)
                        {
                            Cache.Remove(chunkCoord);
                        }
                        Log.debug($"Unloaded {chunksToRemove.Count()} / {loadedChunks} chunks from '{LevelName}' ({Dimension}). {Cache.Count()} still loaded.");
                    }
                }
                await Task.Delay(20000);
                UnloadChunks();
            });
        }

        public Block GetBlock(Vector3 playerPos)
        {
            return GetBlock((int)(playerPos.X < 0 ? playerPos.X - 1 : playerPos.X), (int)playerPos.Y - 3, (int)(playerPos.Z < 0 ? playerPos.Z - 1 : playerPos.Z));
        }

        public Block GetBlock(int x, int y, int z)
        {
            int chunkX = (int)Math.Floor(x / 16f);
            int chunkZ = (int)Math.Floor(z / 16f);

            if (Cache.TryGetValue((chunkX, chunkZ), out Chunk chunk))
            {
                int subChunkY = (int)Math.Floor((y - MinY) / 16f);

                if (subChunkY < 0) { return new Air(); }

                int localX = ((int)x % 16 + 16) % 16;
                int localY = ((int)y % 16 + 16) % 16;
                int localZ = ((int)z % 16 + 16) % 16;

                if (subChunkY >= chunk.Chunks.Count) { return new Air(); }

                var subChunk = chunk.Chunks[subChunkY];

                if (subChunk == null) { return new Air(); }

                int blockIndex = (localY) | (localZ << 4) | (localX << 8);
                int blockStateIndex = subChunk.Blocks[blockIndex];

                var nbt = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = false,
                    RootTag = subChunk.Palette[blockStateIndex],
                };

                byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);
                var blockHash = Fnv1aHash.Hash32(saveToBuffer);

                if (BlockPalette.blockHashes.TryGetValue(blockHash, out Block value))
                {
                    return value;
                }
                else
                {
                    Log.warn($"Blockstate hash {blockHash} not found in BlockPalette. Requested state: {subChunk.Palette[blockStateIndex]}");
                    return new Air();
                }
            }
            else
            {
                Log.warn($"Tried to access unloaded chunk x:{chunkX}; z:{chunkZ}. In '{LevelName}' position x:{x}; y:{y}, z:{z}");
                return new Air();
            }
        }

        public void Unload()
        {
            if (Db == null || Overworld != this) //Nether and End share the Overworld database
            {
                return;
            }
            Log.info($"Unloading world: {LevelName}.mcworld");
            Db.Dispose();
            Db.Close();
        }
    }
}
