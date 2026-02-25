namespace DaemonMC.Network.Bedrock
{
    public class ClientboundDataDrivenUIShowScreen : Packet
    {
        public override int Id => (int) Info.Bedrock.ClientboundDataDrivenUIShowScreen;

        public string ScreenID { get; set; } = "";
        public int FormID { get; set; } = 0;
        public int InstanceId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(ScreenID);
            encoder.WriteInt(FormID);
            encoder.WriteOptional(InstanceId == 0 ? null : () => encoder.WriteInt(InstanceId));
        }
    }
}
