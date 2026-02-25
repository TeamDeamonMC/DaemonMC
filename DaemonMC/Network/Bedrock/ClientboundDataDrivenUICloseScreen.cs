namespace DaemonMC.Network.Bedrock
{
    public class ClientboundDataDrivenUICloseScreen : Packet
    {
        public override int Id => (int) Info.Bedrock.ClientboundDataDrivenUICloseScreen;

        public int FormID { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteOptional(FormID == 0 ? null : () => encoder.WriteInt(FormID));
        }
    }
}
