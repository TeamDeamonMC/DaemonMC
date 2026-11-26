using DaemonMC.Level;
using DaemonMC.Network;
using DaemonMC.Utils.Text;

namespace DaemonMC.Biomes
{
    public class BiomeManager
    {
        private static readonly object registryLock = new object();
        private static readonly Dictionary<string, Biome> byName = new Dictionary<string, Biome>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<int, Biome> byId = new Dictionary<int, Biome>();
        private static List<Biome> ordered = new List<Biome>();

        static BiomeManager()
        {
            foreach (var biome in VanillaBiomes.Create())
            {
                Register(biome);
            }
        }

        public static List<Biome> Biomes
        {
            get { lock (registryLock) { return ordered; } }
        }

        public static void Register(Biome biome)
        {
            if (string.IsNullOrWhiteSpace(biome.BiomeName))
            {
                throw new ArgumentException("Biome must have a name");
            }
            biome.BiomeName = StripNamespace(biome.BiomeName);

            lock (registryLock)
            {
                if (byId.TryGetValue(biome.Id, out var existing) && !existing.BiomeName.Equals(biome.BiomeName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Biome id {biome.Id} is already used by {existing.BiomeName}");
                }
                if (byName.TryGetValue(biome.BiomeName, out var old))
                {
                    byId.Remove(old.Id);
                }
                byName[biome.BiomeName] = biome;
                byId[biome.Id] = biome;
                ordered = byId.Values.OrderBy(b => b.Id).ToList();
            }
            PacketCache.Invalidate();
        }

        public static Biome? Get(string biomeName)
        {
            lock (registryLock)
            {
                return byName.GetValueOrDefault(StripNamespace(biomeName));
            }
        }

        public static Biome? Get(int biomeId)
        {
            lock (registryLock)
            {
                return byId.GetValueOrDefault(biomeId);
            }
        }

        public static int GetBiomeId(string biomeName)
        {
            var biome = Get(biomeName);
            if (biome != null)
            {
                return biome.Id;
            }
            Log.error($"Couldn't get {biomeName} id");
            return -1;
        }

        public static Biome GetDefault(Dimension dimension)
        {
            string name = dimension switch
            {
                Dimension.Nether => "hell",
                Dimension.End => "the_end",
                _ => "plains"
            };
            return Get(name) ?? Get(1) ?? Biomes.First();
        }

        private static string StripNamespace(string name) => name.StartsWith("minecraft:", StringComparison.OrdinalIgnoreCase) ? name.Substring(10) : name;
    }

    public class Biome
    {
        public string BiomeName { get; set; } = "";
        public int Id { get; set; }
        public BiomeDefinitionData BiomeData { get; set; } = new BiomeDefinitionData();

        public override string ToString() => $"{BiomeName} ({Id})";
    }

    public class BiomeDefinitionData
    {
        public short BiomeID { get; set; } = -1;
        public float Temperature { get; set; } = 0;
        public float Downfall { get; set; } = 0;
        public float FoliageSnow { get; set; } = 0;
        public float RedSporeDensity { get; set; } = 0;
        public float BlueSporeDensity { get; set; } = 0;
        public float AshDensity { get; set; } = 0;
        public float WhiteAshDensity { get; set; } = 0;
        public float Depth { get; set; } = 0;
        public float Scale { get; set; } = 0;
        public int WaterColor { get; set; } = 0;
        public bool Rain { get; set; } = false;
        public List<string> Tags { get; set; } = new List<string>();
    }
}
