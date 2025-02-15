using System.IO.Compression;
using DaemonMC.Entities;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;
using fNbt;
using MiNET.LevelDB;

namespace DaemonMC.Level
{
    public class World
    {
        public bool temporary;
        public string levelName;
        public Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();
        public Dictionary<long, Entity> Entities = new Dictionary<long, Entity>();
        public Dictionary<string, GameRule> GameRules = new Dictionary<string, GameRule>();
        public Database db;
        public string LevelDisplayName { get; set; } = "DaemonMC Temp World";
        public int Version { get; set; } = Info.protocolVersion.Last();
        public int spawnX { get; set; } = 0;
        public int spawnZ { get; set; } = 0;
        public long RandomSeed { get; set; } = 0;

        public World(string LevelName)
        {
            levelName = LevelName;
            load();
        }

        public void addPlayer(Player player)
        {
            foreach (Player onlinePlayer in onlinePlayers.Values)
            {
                PacketEncoder encoder3 = PacketEncoderPool.Get(onlinePlayer);
                var packet3 = new PlayerList
                {
                    UUID = player.UUID,
                    EntityId = player.EntityID,
                    Username = player.Username,
                    XUID = player.XUID,
                    Skin = player.Skin
                };
                packet3.Encode(encoder3);

                PacketEncoder encoder4 = PacketEncoderPool.Get(player);
                var packet4 = new PlayerList
                {
                    UUID = onlinePlayer.UUID,
                    EntityId = onlinePlayer.EntityID,
                    Username = onlinePlayer.Username,
                    XUID = onlinePlayer.XUID,
                    Skin = onlinePlayer.Skin
                };
                packet4.Encode(encoder4);

                if (onlinePlayer == player) { continue; }

                PacketEncoder encoder = PacketEncoderPool.Get(onlinePlayer);
                var packet = new AddPlayer
                {
                    UUID = player.UUID,
                    Username = player.Username,
                    EntityId = player.EntityID,
                    Position = player.Position,
                    Metadata = player.metadata
                };
                packet.Encode(encoder);

                PacketEncoder encoder2 = PacketEncoderPool.Get(player);
                var packet2 = new AddPlayer
                {
                    UUID = onlinePlayer.UUID,
                    Username = onlinePlayer.Username,
                    EntityId = onlinePlayer.EntityID,
                    Position = onlinePlayer.Position,
                    Metadata = onlinePlayer.metadata
                };
                packet2.Encode(encoder2);
            }
        }

        public void removePlayer(Player player)
        {
            if (!onlinePlayers.Remove(player.EntityID))
            {
                Log.warn($"Couldn't despawn {player.Username} from World {player.currentLevel.levelName}.");
            }
            else
            {
                foreach (var dest in onlinePlayers.Values)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(dest);
                    var pk = new RemoveActor
                    {
                        EntityId = player.EntityID
                    };
                    pk.Encode(encoder);
                }
                Log.debug($"Despawned {player.Username} from World {player.currentLevel.levelName}");
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
                byte[] subChunk = db.Get(dataKey);

                if (subChunk != null)
                {
                    try
                    {
                        chunk.chunks.Add(ChunkUtils.DecodeSubChunk(subChunk));
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

        public void load()
        {
            var tempData = Path.Combine(Path.GetTempPath(), $"{levelName}.mcworld");
            if (Directory.Exists(tempData))
            {
                Directory.Delete(tempData, true);
            }
            if (File.Exists($"Worlds/{levelName}.mcworld"))
            {
                Log.info($"Loading world: Worlds\\{levelName}.mcworld");

                using (ZipArchive archive = ZipFile.OpenRead($"Worlds/{levelName}.mcworld"))
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
                                    spawnX = tag["SpawnX"].IntValue;
                                    spawnZ = tag["SpawnZ"].IntValue;

                                    if (tag != null && tag["lastOpenedWithVersion"] is NbtList versionList)
                                    {
                                        string stringVersion = string.Join(".", versionList.Take(3).Select(v => ((NbtInt)v).IntValue));
                                        if (stringVersion != Info.version)
                                        {
                                            Log.error("Unsupported world version!");
                                            Log.warn($"This server software doesn't support world format updating, please open Worlds/{levelName}.mcworld with Minecraft client {Info.version} and export mcworld file again to update world.");
                                            Server.crash = true;
                                        }
                                    }

                                    GameRules.Add("showCoordinates", new GameRule(true));
                                }
                            }
                        }
                    }
                }

                db = new Database(new DirectoryInfo($"Worlds/{levelName}.mcworld"));
                db.Open();
            }
            else
            {
                Log.warn($"World Worlds/{levelName}.mcworld not found. Generating temporary flat world...");
                GameRules.Add("showCoordinates", new GameRule(true));
                temporary = true;
            }
        }

        public void unload()
        {
            if (temporary)
            {
                return;
            }
            Log.info($"Unloading world: {levelName}.mcworld");
            db.Dispose();
            db.Close();
        }
    }
}
