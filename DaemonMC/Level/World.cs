using System.IO.Compression;
using System.Numerics;
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
        public bool Temporary { get; set; } = false;
        public string LevelName { get; set; } = "";
        public Dictionary<long, Player> OnlinePlayers { get; set; } = new Dictionary<long, Player>();
        public Dictionary<long, Entity> Entities { get; set; } = new Dictionary<long, Entity>();
        public Dictionary<string, GameRule> GameRules { get; set; } = new Dictionary<string, GameRule>();
        public Database Db { get; set; }
        public string LevelDisplayName { get; set; } = "DaemonMC Temp World";
        public int Version { get; set; } = Info.ProtocolVersion.Last();
        public int SpawnX { get; set; } = 0;
        public int SpawnZ { get; set; } = 0;
        public long RandomSeed { get; set; } = 0;

        public World(string levelName)
        {
            LevelName = levelName;
            Load();
        }

        public void Send(Packet packet)
        {
            foreach (var dest in OnlinePlayers)
            {
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

        public void AddPlayer(Player player)
        {
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
                        Metadata = entity.Metadata
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
                    Layers = player.Abilities
                };
                onlinePlayer.Send(packet);

                var packet2 = new AddPlayer
                {
                    UUID = onlinePlayer.UUID,
                    Username = onlinePlayer.Username,
                    EntityId = onlinePlayer.EntityID,
                    Position = onlinePlayer.Position,
                    Metadata = onlinePlayer.Metadata,
                    Layers = onlinePlayer.Abilities
                };
                player.Send(packet2);
            }
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
            byte[] index = ToDataTypes.GetByteSum(BitConverter.GetBytes(x), BitConverter.GetBytes(z));
            byte[] dataKey = ToDataTypes.GetByteSum(index, new byte[] { 0x2f, 0 });

            var chunk = new Chunk();

            for (int y = 0; y < 24; y++) //since 1.18 320 up, -64 down. 64/16=4 negative subchunks. 320/16=20 positive subchunks. max 24 chunks.
            {
                dataKey[^1] = (byte)(y - 4);
                byte[] subChunk = Db.Get(dataKey);

                if (subChunk != null)
                {
                    try
                    {
                        chunk.Chunks.Add(ChunkUtils.DecodeSubChunk(subChunk));
                    }
                    catch(Exception ex)
                    {
                        Log.error($"Chunk x:{x}; z:{z} decoding failed. Fix this. Sent empty chunk");
                        Log.error(ex.ToString());
                        return chunk;
                    }
                }
                else
                {
                    return chunk;
                }
            }
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

                                    if (tag != null && tag["lastOpenedWithVersion"] is NbtList versionList)
                                    {
                                        string stringVersion = string.Join(".", versionList.Take(3).Select(v => ((NbtInt)v).IntValue));
                                        if (stringVersion != Info.Version)
                                        {
                                            Log.error($"Unsupported world version {stringVersion}!");
                                            Log.warn($"This server software doesn't support world format updating, please open Worlds/{LevelName}.mcworld with Minecraft client {Info.Version} and export mcworld file again to update world.");
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
            }
            else
            {
                Log.warn($"World Worlds/{LevelName}.mcworld not found. Generating temporary flat world...");
                GameRules.Add("showCoordinates", new GameRule(true));
                Temporary = true;
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
