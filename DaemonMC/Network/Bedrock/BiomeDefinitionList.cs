using DaemonMC.Biomes;

namespace DaemonMC.Network.Bedrock
{
    public class BiomeDefinitionList : Packet
    {
        public override int Id => (int) Info.Bedrock.BiomeDefinitionList;

        public List<Biome> BiomeData { get; set; } = new List<Biome>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBiomes(BiomeData);
        }
    }
}
