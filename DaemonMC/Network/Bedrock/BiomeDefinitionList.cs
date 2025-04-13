using DaemonMC.Biomes;

namespace DaemonMC.Network.Bedrock
{
    public class BiomeDefinitionList : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.BiomeDefinitionList;

        public List<Biome> BiomeData { get; set; } = new List<Biome>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (encoder.protocolVersion >= Info.v1_21_80)
            {
                encoder.WriteBiomes(BiomeData);
            }
            else
            {
                encoder.WriteBiomesOld(BiomeData);
            }
        }
    }
}
