namespace DaemonMC.Network.Bedrock
{
    public class SetLocalPlayerAsInitialized : Packet
    {
        public override int Id => (int) Info.Bedrock.SetLocalPlayerAsInitialized;

        public long EntityID { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            EntityID = decoder.ReadVarLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
