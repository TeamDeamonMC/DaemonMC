﻿using DaemonMC.Utils.Text;

namespace DaemonMC.Biomes
{
    public class BiomeManager
    {
        public static List<Biome> Biomes { get; set; } = new List<Biome>() {
            new Biome() { BiomeName = "plains", BiomeData = new BiomeDefinitionData() { Downfall = 0.5f, Depth = 0.125f, Scale = 0.05f } }
        };

        public static int GetBiomeId(string biomeName)
        {
            var biome = Biomes.FirstOrDefault(b => b.BiomeName.Equals(biomeName, StringComparison.OrdinalIgnoreCase));
            if (biome != null)
            {
                return biome.BiomeData.BiomeID;
            }
            Log.error($"Couldn't get {biomeName} id");
            return -1;
        }
    }

    public class Biome
    {
        public string BiomeName { get; set; } = "";
        public BiomeDefinitionData BiomeData { get; set; } = new BiomeDefinitionData();

    }

    public class BiomeDefinitionData
    {
        public short BiomeID { get; set; } = -1;
        public float Temperature { get; set; } = 0;
        public float Downfall { get; set; } = 0;
        public float RedSporeDensity { get; set; } = 0;
        public float BlueSporeDensity { get; set; } = 0;
        public float AshDensity { get; set; } = 0;
        public float WhiteAshDensity { get; set; } = 0;
        public float Depth { get; set; } = 0;
        public float Scale { get; set; } = 0;
        public int WaterColor { get; set; } = 0;
        public bool Rain { get; set; } = false;
    }
}
