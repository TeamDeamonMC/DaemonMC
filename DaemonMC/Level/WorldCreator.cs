using System.IO.Compression;
using DaemonMC.Level.Generators;
using DaemonMC.Network;
using fNbt;
using MiNET.LevelDB;
using Newtonsoft.Json;

namespace DaemonMC.Level
{
    public static class WorldCreator
    {
        private const int StorageVersion = 10;
        private const int FlatGenerator = 2;

        public static void Create(string mcworldPath, string levelName, SuperFlat generator)
        {
            string stagingDir = Path.Combine(Path.GetTempPath(), $"DaemonMC-create-{Guid.NewGuid():N}");

            try
            {
                Directory.CreateDirectory(stagingDir);

                File.WriteAllBytes(Path.Combine(stagingDir, "level.dat"), CreateLevelDat(levelName, generator));
                File.WriteAllText(Path.Combine(stagingDir, "levelname.txt"), levelName);

                using (var db = new Database(new DirectoryInfo(Path.Combine(stagingDir, "db")), true))
                {
                    db.Open();
                    db.Close();
                }

                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(mcworldPath))!);
                ZipFile.CreateFromDirectory(stagingDir, mcworldPath);
            }
            finally
            {
                if (Directory.Exists(stagingDir))
                {
                    Directory.Delete(stagingDir, true);
                }
            }
        }

        public static byte[] CreateLevelDat(string levelName, SuperFlat generator)
        {
            int[] version = ToVersionArray(World.LevelDbVersion.Last());

            var root = new NbtCompound("")
            {
                new NbtInt("StorageVersion", StorageVersion),
                new NbtInt("NetworkVersion", Info.ProtocolVersion.Last()),
                new NbtString("LevelName", levelName),
                new NbtLong("RandomSeed", Random.Shared.NextInt64()),
                new NbtInt("Generator", FlatGenerator),
                new NbtString("FlatWorldLayers", CreateFlatWorldLayers(generator)),
                new NbtInt("GameType", DaemonMC.GameMode),
                new NbtInt("Difficulty", 1),
                new NbtInt("SpawnX", 0),
                new NbtInt("SpawnY", 32767),
                new NbtInt("SpawnZ", 0),
                new NbtLong("Time", 0),
                new NbtLong("LastPlayed", DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                new NbtString("InventoryVersion", Info.Version),
                new NbtList("MinimumCompatibleClientVersion", version.Select(v => new NbtInt(v)), NbtTagType.Int),
                new NbtList("lastOpenedWithVersion", version.Select(v => new NbtInt(v)), NbtTagType.Int),
                new NbtByte("showcoordinates", 1),
            };

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = root,
            };
            byte[] body = nbt.SaveToBuffer(NbtCompression.None);

            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(StorageVersion);
            writer.Write(body.Length);
            writer.Write(body);
            return stream.ToArray();
        }

        private static string CreateFlatWorldLayers(SuperFlat generator)
        {
            var layers = new List<object>();
            foreach (var block in generator.Layers)
            {
                if (layers.Count > 0 && layers[^1] is Dictionary<string, object> last && (string)last["block_name"] == block.Name)
                {
                    last["count"] = (int)last["count"] + 1;
                }
                else
                {
                    layers.Add(new Dictionary<string, object> { ["block_name"] = block.Name, ["count"] = 1 });
                }
            }

            return JsonConvert.SerializeObject(new Dictionary<string, object>
            {
                ["biome_id"] = 1,
                ["block_layers"] = layers,
                ["encoding_version"] = 6,
                ["world_version"] = "version.post_1_18",
            }) + "\n";
        }

        internal static int[] ToVersionArray(string version)
        {
            var parts = version.Split('.').Select(int.Parse).ToList();
            while (parts.Count < 5)
            {
                parts.Add(0);
            }
            return parts.Take(5).ToArray();
        }
    }
}
