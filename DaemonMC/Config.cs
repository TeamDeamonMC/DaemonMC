using DaemonMC.Utils.Text;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace DaemonMC
{
    public class Config
    {
        public string serverName { get; set; } = "DaemonMC";
        public string worldName { get; set; } = "Nice new server";
        public string maxOnline { get; set; } = "10";
        public bool debug { get; set; } = false;
        public string spawnWorld { get; set; } = "My World";
        public bool forcePacks { get; set; } = false;

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

            Log.debugMode = config.debug;
            DaemonMC.servername = config.serverName;
            DaemonMC.worldname = config.worldName;
            DaemonMC.maxOnline = config.maxOnline;
            DaemonMC.defaultWorld = config.spawnWorld;
            ResourcePackManager.ForcePacks = config.forcePacks;
        }
    }
}
