namespace DaemonMC.Network.Bedrock
{
    public class ItemRegistry : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ItemRegistry;

        //public itemData Items;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(0); //todo send item data
        }
    }
}
