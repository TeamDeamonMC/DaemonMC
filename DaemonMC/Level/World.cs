using System.Collections.Concurrent;
using System.IO.Compression;
using System.Numerics;
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
        // Loaded chunk columns (block entities, height maps, biomes). no block data.
        public ConcurrentDictionary<(int x, int z), ChunkColumn> Columns { get; } = new ConcurrentDictionary<(int x, int z), ChunkColumn>();
        //Cached sub chunks
        public ConcurrentDictionary<(int x, int y, int z), SubChunk> SubChunks { get; } = new ConcurrentDictionary<(int x, int y, int z), SubChunk>();
        private readonly object loadLock = new object();
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

            if (y < MinY || y > MaxY)
            {
                Log.warn($"Tried to set block outside of world height. In '{LevelName}' ({Dimension}) position x:{x}; y:{y}, z:{z}");
                return;
            }

            if (!BlockPalette.blockHashes.ContainsKey(block.GetHash()))
            {
                Log.warn($"Blockstate hash for {block.Name} not found in BlockPalette. Requested state: {JsonConvert.SerializeObject(block.States, Formatting.Indented)}");
                return;
            }

            int subChunkY = y >> 4;
            ChunkColumn column;
            lock (loadLock)
            {
                column = GetColumn(chunkX, chunkZ);
                var subChunk = GetSubChunk(chunkX, subChunkY, chunkZ)!;
                subChunk.SetBlockState(x & 15, y & 15, z & 15, block.GetState());

                column.HighestSubChunk = Math.Max(column.HighestSubChunk, subChunkY - (MinY >> 4));
                column.Heights = null;
            }
            column.RemoveBlockEntity(x, y, z);
        }

        public NbtCompound? GetBlockEntity(int x, int y, int z)
        {
            return GetColumn(x >> 4, z >> 4).GetBlockEntity(x, y, z);
        }

        public bool SetBlockEntity(NbtCompound blockEntity, bool send = true)
        {
            if (!ChunkColumn.TryGetPosition(blockEntity, out int x, out int y, out int z))
            {
                Log.warn($"Tried to set block entity without x, y, z position in '{LevelName}'");
                return false;
            }

            GetColumn(x >> 4, z >> 4).SetBlockEntity(blockEntity);

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
            return GetColumn(x >> 4, z >> 4).RemoveBlockEntity(x, y, z);
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

        public ChunkColumn GetColumn(int x, int z)
        {
            if (Columns.TryGetValue((x, z), out var cached))
            {
                return cached;
            }

            lock (loadLock)
            {
                if (Columns.TryGetValue((x, z), out cached))
                {
                    return cached;
                }

                var column = new ChunkColumn(x, z, Dimension);
                int minSubChunkIndex = MinY >> 4;

                byte[] dataKey = SubChunkKey(x, z);
                for (int i = Dimension.SubChunkCount() - 1; i >= 0; i--)
                {
                    dataKey[^1] = unchecked((byte)(i + minSubChunkIndex));
                    byte[] data = Db.Get(dataKey);
                    if (data != null)
                    {
                        column.HighestSubChunk = i;
                        SubChunks[(x, i + minSubChunkIndex, z)] = DecodeSubChunk(data, x, i + minSubChunkIndex, z);
                        break;
                    }
                }

                if (column.HighestSubChunk == -1)
                {
                    column.Generated = true;
                    column.HighestSubChunk = Generator.SubChunkCount - 1;
                }

                byte[] blockEntityData = Db.Get(ToDataTypes.GetByteSum(ChunkKey(x, z), new byte[] { 0x31 }));
                if (blockEntityData != null)
                {
                    try
                    {
                        column.LoadBlockEntities(blockEntityData);
                    }
                    catch (Exception ex)
                    {
                        Log.error($"Block entities in chunk x:{x}; z:{z} in '{LevelName}' could not be loaded");
                        Log.error(ex.ToString());
                    }
                }

                Columns[(x, z)] = column;

                if (EntityChunksLoaded.Add((x, z)))
                {
                    LoadEntities(x, z);
                }

                return column;
            }
        }

        public SubChunk? GetSubChunk(int x, int subChunkY, int z)
        {
            int index = subChunkY - (MinY >> 4);
            if (index < 0 || index >= Dimension.SubChunkCount())
            {
                return null;
            }

            if (SubChunks.TryGetValue((x, subChunkY, z), out var cached))
            {
                return cached;
            }

            lock (loadLock)
            {
                var column = GetColumn(x, z);

                if (SubChunks.TryGetValue((x, subChunkY, z), out cached))
                {
                    return cached;
                }

                SubChunk subChunk;
                if (index > column.HighestSubChunk)
                {
                    subChunk = new SubChunk();
                }
                else if (column.Generated)
                {
                    subChunk = Generator.GenerateSubChunk(index);
                }
                else
                {
                    byte[] dataKey = SubChunkKey(x, z);
                    dataKey[^1] = unchecked((byte)subChunkY);
                    byte[] data = Db.Get(dataKey);
                    subChunk = data != null ? DecodeSubChunk(data, x, subChunkY, z) : new SubChunk();
                }

                SubChunks[(x, subChunkY, z)] = subChunk;
                return subChunk;
            }
        }

        public int[] GetColumnHeights(int x, int z)
        {
            var column = GetColumn(x, z);
            var heights = column.Heights;
            if (heights != null)
            {
                return heights;
            }

            lock (loadLock)
            {
                int minSubChunkIndex = MinY >> 4;
                heights = column.Heights ?? ChunkColumn.ComputeHeights(column.HighestSubChunk, i => GetSubChunk(x, i + minSubChunkIndex, z));
                column.Heights = heights;
                return heights;
            }
        }

        public int GetHighestNonAirSubChunk(int x, int z)
        {
            return ChunkColumn.HighestNonAirSubChunk(GetColumnHeights(x, z));
        }

        public int GetLandingSubChunk(Vector3 position)
        {
            int x = (int)Math.Floor(position.X);
            int y = (int)Math.Floor(position.Y);
            int z = (int)Math.Floor(position.Z);

            int height = GetColumnHeights(x >> 4, z >> 4)[((z & 15) << 4) | (x & 15)];
            int subChunkY;
            if (height < 0)
            {
                subChunkY = MinY >> 4;
            }
            else
            {
                int ground = MinY + height;
                subChunkY = (ground <= y ? ground : Math.Clamp(y, MinY, MaxY)) >> 4;
            }

            GetSubChunk(x >> 4, subChunkY, z >> 4);
            return subChunkY;
        }

        private byte[] SubChunkKey(int x, int z)
        {
            return ToDataTypes.GetByteSum(ChunkKey(x, z), new byte[] { 0x2f, 0 });
        }

        private SubChunk DecodeSubChunk(byte[] data, int x, int subChunkY, int z)
        {
            try
            {
                return ChunkUtils.DecodeSubChunk(data);
            }
            catch (Exception ex)
            {
                Log.error($"SubChunk x:{x}; y:{subChunkY} z:{z} in '{LevelName}' ({Dimension}) decoding failed. This shouldn't happen. Using empty sub chunk");
                Log.error(ex.ToString());
                return new SubChunk();
            }
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
            var spawnColumn = GetColumn(SpawnX >> 4, SpawnZ >> 4);
            if (spawnColumn.HighestSubChunk < 0)
            {
                SpawnY = MaxY;
                Log.warn($"Unable to calulate spawn point Y value for {LevelName} ({Dimension}). Spawn chunk not found in DB. Y value set to {SpawnY}");
            }
            else
            {
                SpawnY = Math.Min(MinY + (spawnColumn.HighestSubChunk + 1) * 16, MaxY);
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
                if (!SubChunks.IsEmpty || !Columns.IsEmpty)
                {
                    //Area around each player that stays cached: view distance in every direction (chunk x/z and absolute sub chunk y).
                    var activeAreas = new List<(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)>();
                    foreach (var player in OnlinePlayers.Values)
                    {
                        int chunkX = (int)Math.Floor(player.Position.X / 16.0);
                        int subChunkY = (int)Math.Floor(player.Position.Y / 16.0);
                        int chunkZ = (int)Math.Floor(player.Position.Z / 16.0);
                        int distance = player.drawDistance / 2;

                        activeAreas.Add((chunkX - distance, chunkX + distance, subChunkY - distance, subChunkY + distance, chunkZ - distance, chunkZ + distance));
                    }

                    var spawnChunk = (SpawnX >> 4, SpawnZ >> 4);
                    activeAreas.Add((spawnChunk.Item1, spawnChunk.Item1, MinY >> 4, MaxY >> 4, spawnChunk.Item2, spawnChunk.Item2));
                    int removedSubChunks = 0;

                    lock (loadLock)
                    {
                        foreach (var key in SubChunks.Keys)
                        {
                            bool active = activeAreas.Exists(a => key.x >= a.minX && key.x <= a.maxX && key.y >= a.minY && key.y <= a.maxY && key.z >= a.minZ && key.z <= a.maxZ);
                            if (!active && SubChunks.TryRemove(key, out _))
                            {
                                removedSubChunks++;
                            }
                        }

                        var usedColumns = SubChunks.Keys.Select(k => (k.x, k.z)).ToHashSet();
                        foreach (var chunkCoord in Columns.Keys)
                        {
                            if (!usedColumns.Contains(chunkCoord) && chunkCoord != spawnChunk)
                            {
                                Columns.TryRemove(chunkCoord, out _);
                            }
                        }
                    }

                    if (removedSubChunks != 0)
                    {
                        Log.debug($"Unloaded {removedSubChunks} sub chunks from '{LevelName}' ({Dimension}). {SubChunks.Count} sub chunks still loaded.");
                    }
                }
                await Task.Delay(30000);
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

            var subChunk = GetSubChunk(chunkX, y >> 4, chunkZ);
            var state = subChunk?.GetBlockState(x & 15, y & 15, z & 15);

            if (state == null) { return new Air(); }

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = state,
            };

            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);
            var blockHash = Fnv1aHash.Hash32(saveToBuffer);

            if (BlockPalette.blockHashes.TryGetValue(blockHash, out Block value))
            {
                return value;
            }
            else
            {
                Log.warn($"Blockstate hash {blockHash} not found in BlockPalette. Requested state: {state}");
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
