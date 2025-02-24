using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class BiomeDefinitionList : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.BiomeDefinitionList;

        public NbtCompound BiomeData { get; set; } = new NbtCompound();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteCompoundTag(BiomeData);
        }
    }
}
