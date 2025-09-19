using DaemonMC.Utils.Text;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using DaemonMC.Utils;

namespace DaemonMC
{
    public class Config
    {
        public string ServerName { get; set; } = "DaemonMC";
        public string WorldName { get; set; } = "Nice new server";
        public string SpawnWorld { get; set; } = "My World";
        public string MaxOnline { get; set; } = "10";
        public string DefaultGamemode { get; set; } = "Survival";
        public int DrawDistance { get; set; } = 10;
        public bool UnloadChunks { get; set; } = true;
        public int Port { get; set; } = 19132;
        public bool ForcePacks { get; set; } = false;
        public bool XboxAuth { get; set; } = true;
        public string Compression { get; set; } = "none";
        public bool Debug { get; set; } = false;
        public bool Encryption { get; set; } = false; //todo set true when tested that its stable

        public static void Set()
        {
            string configFile = "DaemonMC.yaml";

            Config config = new Config();

            if (!File.Exists(configFile))
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                string yamlContent = serializer.Serialize(config);
                File.WriteAllText(configFile, yamlContent);

                Log.warn($"Configuration file '{configFile}' was not found. New '{configFile}' been created with default values.");
            }
            else
            {
                Log.info("Loading config: DaemonMC.yaml");
                string fileContent = File.ReadAllText(configFile);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                config = deserializer.Deserialize<Config>(fileContent);

            }

            Log.debugMode = config.Debug;
            DaemonMC.Servername = config.ServerName;
            DaemonMC.Worldname = config.WorldName;
            DaemonMC.MaxOnline = config.MaxOnline;
            DaemonMC.DefaultWorld = config.SpawnWorld;
            DaemonMC.GameMode = ToGameMode(config.DefaultGamemode);
            DaemonMC.DrawDistance = config.DrawDistance;
            DaemonMC.UnloadChunks = config.UnloadChunks;
            DaemonMC.Compression = ToCompressionType(config.Compression);
            Server.Port = config.Port;
            JWT.XboxAuth = config.XboxAuth;
            ResourcePackManager.ForcePacks = config.ForcePacks;
            Cryptography.UseEncryption = config.Encryption;
        }

        public static int ToGameMode(string gameMode)
        {
            switch (gameMode.ToLower())
            {
                case "survival":
                    return 0;
                case "adventure":
                    return 1;
                case "creative":
                    return 2;
                default:
                    Log.warn($"Unknown GameMode {gameMode}. Check DaemonMC.yaml.");
                    return 2;
            }
        }

        public static CompressionTypes ToCompressionType(string type)
        {
            switch (type.ToLower())
            {
                case "none":
                    return CompressionTypes.None;
                case "zlib":
                    return CompressionTypes.ZLib;
                case "snappy":
                    return CompressionTypes.Snappy;
                default:
                    Log.warn($"Unknown Compression Types {type}. Check DaemonMC.yaml.");
                    return CompressionTypes.None;
            }
        }
    }
}
