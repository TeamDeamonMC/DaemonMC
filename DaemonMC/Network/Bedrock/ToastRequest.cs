namespace DaemonMC.Network.Bedrock
{
    public class ToastRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.ToastRequest;

        public string Title { get; set; } = "";
        public string Body { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(Title);
            encoder.WriteString(Body);
        }
    }
}
