using System.IO.Compression;
using DaemonMC.Network;
using DaemonMC.Utils.Text;
using fNbt;

namespace DaemonMC.Level
{
    public class World
    {
        public bool temporary;
        public string levelName;
        public LevelDBInterface levelDB = new LevelDBInterface();
        public static Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();
        public string LevelDisplayName { get; set; } = "DaemonMC Temp World";
        public int Version { get; set; } = Info.protocolVersion;
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
            onlinePlayers.Add(player.EntityID, player);
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
