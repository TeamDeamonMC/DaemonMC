using System.IO.Compression;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils.Text;
using fNbt;

namespace DaemonMC.Level
{
    public class World
    {
        public bool temporary;
        public string levelName;
        public LevelDBInterface levelDB = new LevelDBInterface();
        public Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();
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
                PacketEncoder encoder = PacketEncoderPool.Get(onlinePlayer);
                var packet = new AddPlayer
                {
                    Username = player.Username,
                    EntityId = player.EntityID,
                    Position = player.Position,
                    Metadata = player.metadata
                };
                packet.Encode(encoder);

                PacketEncoder encoder2 = PacketEncoderPool.Get(player);
                var packet2 = new AddPlayer
                {
                    Username = onlinePlayer.Username,
                    EntityId = onlinePlayer.EntityID,
                    Position = onlinePlayer.Position,
                    Metadata = onlinePlayer.metadata
                };
                packet2.Encode(encoder2);
            }

            onlinePlayers.Add(player.EntityID, player);
        }

        public void removePlayer(Player player)
        {
            if (!onlinePlayers.Remove(player.EntityID))
            {
                Log.warn($"Couldn't despawn {player.Username} from World {player.currentLevel.levelName}.");
            }
            else
            {
                Log.debug($"Despawned {player.Username} from World {player.currentLevel.levelName}");
            }
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
                Log.info($"Loading world: {levelName}.mcworld");

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
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Log.warn($"World Worlds/{levelName}.mcworld not found. Generating temporary flat world...");
                temporary = true;
            }
        }
    }
}
