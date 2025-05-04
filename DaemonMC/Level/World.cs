using System.IO.Compression;
using System.Numerics;
using DaemonMC.Biomes;
using DaemonMC.Blocks;
using DaemonMC.Entities;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;
using fNbt;
using MiNET.LevelDB;

namespace DaemonMC.Level
{
    public class World
    {
        private string LevelDbVersion { get; set; } = "1.21.70";
        public Dictionary<(int x, int z), Chunk> Cache { get; set; } = new Dictionary<(int x, int z), Chunk>();
        public bool Temporary { get; set; } = false;
        public string LevelName { get; set; } = "";
        public Dictionary<long, Player> OnlinePlayers { get; set; } = new Dictionary<long, Player>();
        public Dictionary<long, Entity> Entities { get; set; } = new Dictionary<long, Entity>();
        public Dictionary<string, GameRule> GameRules { get; set; } = new Dictionary<string, GameRule>();
        public Database Db { get; set; }
        public string LevelDisplayName { get; set; } = "DaemonMC Temp World";
        public int Version { get; set; } = Info.ProtocolVersion.Last();
        public int SpawnX { get; set; } = 0;
        public int SpawnY { get; set; } = 384;
        public int SpawnZ { get; set; } = 0;
        public long RandomSeed { get; set; } = 0;

        public World(string levelName)
        {
            LevelName = levelName;
            Load();
            UnloadChunks();
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
            Log.info($"{player.Username} spawned in World:'{LevelName}' X:{player.Position.X} Y:{player.Position.Y} Z:{player.Position.Z}");
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

            byte[] index = ToDataTypes.GetByteSum(BitConverter.GetBytes(x), BitConverter.GetBytes(z));
            byte[] dataKey = ToDataTypes.GetByteSum(index, new byte[] { 0x2f, 0 });

            var chunk = new Chunk();

            List<byte[]> subChunks = new List<byte[]>(24);
            int lastSubChunkY = -1;

            for (int y = 0; y < 24; y++) //since 1.18 320 up, -64 down. 64/16=4 negative subchunks. 320/16=20 positive subchunks. max 24 chunks.
            {
                dataKey[^1] = (byte)(y - 4);
                byte[] subChunk = Db.Get(dataKey);

                subChunks.Add(subChunk);
                if (subChunk != null)
                {
                    lastSubChunkY = y;
                }
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

            Cache.Add((x, z), chunk);
            return chunk;
        }

        public void Load()
        {
            var tempData = Path.Combine(Path.GetTempPath(), $"{LevelName}.mcworld");
            if (Directory.Exists(tempData))
            {
                Directory.Delete(tempData, true);
            }
            if (File.Exists($"Worlds/{LevelName}.mcworld"))
            {
                Log.info($"Loading world: Worlds\\{LevelName}.mcworld");

                using (ZipArchive archive = ZipFile.OpenRead($"Worlds/{LevelName}.mcworld"))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith("level.dat", StringComparison.OrdinalIgnoreCase))
                        {
                            using (Stream stream = entry.Open())
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    stream.CopyTo(memoryStream);

                                    memoryStream.Position = 8;

                                    var nbt = new NbtFile
                                    {
                                        BigEndian = false,
                                        UseVarInt = false
                                    };

                                    nbt.LoadFromStream(memoryStream, NbtCompression.None);
                                    NbtTag tag = nbt.RootTag;

                                    RandomSeed = tag["RandomSeed"].LongValue;
                                    LevelDisplayName = tag["LevelName"].StringValue;
                                    Version = tag["NetworkVersion"].IntValue;
                                    SpawnX = tag["SpawnX"].IntValue;
                                    SpawnZ = tag["SpawnZ"].IntValue;

                                    if (tag != null && tag["MinimumCompatibleClientVersion"] is NbtList versionList)
                                    {
                                        string stringVersion = string.Join(".", versionList.Take(3).Select(v => ((NbtInt)v).IntValue));
                                        if (stringVersion != LevelDbVersion)
                                        {
                                            Log.error($"Unsupported world version {stringVersion}!");
                                            Log.warn($"This server software doesn't support world format updating, please open Worlds/{LevelName}.mcworld with Minecraft client {LevelDbVersion} and export mcworld file again to update world.");
                                            Server.Crash = true;
                                        }
                                    }

                                    GameRules.Add("showCoordinates", new GameRule(true));
                                }
                            }
                        }
                    }
                }

                Db = new Database(new DirectoryInfo($"Worlds/{LevelName}.mcworld"));
                Db.Open();

                var SpawnChunk = GetChunk(SpawnX / 16, SpawnZ / 16);
                if (SpawnChunk.Chunks.Count == 0)
                {
                    Log.warn($"Unable to calulate spawn point Y value for {LevelName}. Spawn chunk not found in DB. Y value set to 384");
                }
                else
                {
                    SpawnY = (SpawnChunk.Chunks.Count * 16) - 64;
                }
                Log.debug($"Calculated '{LevelName}' spawn point: X:{SpawnX} Y:{SpawnY} Z:{SpawnZ}");
            }
            else
            {
                Log.warn($"World Worlds/{LevelName}.mcworld not found. Generating temporary flat world...");
                GameRules.Add("showCoordinates", new GameRule(true));
                Temporary = true;
            }
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
                        .Where(chunkCoord => !activeChunks.Contains(chunkCoord) && chunkCoord != (SpawnX / 16, SpawnZ / 16))
                        .ToList();

                    int loadedChunks = Cache.Count();

                    if (chunksToRemove.Count != 0)
                    {
                        foreach (var chunkCoord in chunksToRemove)
                        {
                            Cache.Remove(chunkCoord);
                        }
                        Log.debug($"Unloaded {chunksToRemove.Count()} / {loadedChunks} chunks from '{LevelName}'. {Cache.Count()} still loaded.");
                    }
                }
                await Task.Delay(20000);
                UnloadChunks();
            });
        }

        public Block GetBlock(Vector3 position)
        {
            int posX = (int)(position.X < 0 ? position.X - 1 : position.X);
            int posZ = (int)(position.Z < 0 ? position.Z - 1 : position.Z);

            int chunkX = (int)Math.Floor(posX / 16f);
            int chunkZ = (int)Math.Floor(posZ / 16f);

            if (Cache.TryGetValue((chunkX, chunkZ), out Chunk chunk))
            {
                int subChunkY = (int)Math.Floor((position.Y + 64) / 16f);

                int localX = ((int)posX % 16 + 16) % 16;
                int localY = ((int)position.Y % 16 + 16) % 16;
                int localZ = ((int)posZ % 16 + 16) % 16;

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
                Log.warn($"Tried to access unloaded chunk x:{chunkX}; z:{chunkZ}. In '{LevelName}' position x:{posX}; y:{position.Y}, z:{posZ}");
                return new Air();
            }
        }

        public void Unload()
        {
            if (Temporary)
            {
                return;
            }
            Log.info($"Unloading world: {LevelName}.mcworld");
            Db.Dispose();
            Db.Close();
        }
    }
}
