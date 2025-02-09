namespace DaemonMC.Network.Bedrock
{
    public class ItemRegistry
    {
        public Info.Bedrock id = Info.Bedrock.ItemRegistry;

        //public itemData Items;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarInt(0); //todo send item data
            encoder.handlePacket();
        }
    }
}
