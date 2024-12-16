using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class BiomeDefinitionList
    {
        public Info.Bedrock id = Info.Bedrock.BiomeDefinitionList;

        public NbtCompound biomeData = new NbtCompound();

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteCompoundTag(biomeData);
            encoder.handlePacket();
        }
    }
}
