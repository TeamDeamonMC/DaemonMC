namespace DaemonMC.Network.Bedrock
{
    public class Disconnect : Packet
    {
        public override int Id => (int) Info.Bedrock.Disconnect;

        public int Reason { get; set; } = 0;
        public bool SkipMessage { get; set; } = false;
        public string Message { get; set; } = "";
        public string FilteredMessage { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(Reason);
            encoder.WriteBool(SkipMessage);
            encoder.WriteString(Message);
            encoder.WriteString(FilteredMessage);
        }
    }
}
