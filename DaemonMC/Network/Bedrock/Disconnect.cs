namespace DaemonMC.Network.Bedrock
{
    public class Disconnect : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Disconnect;

        public string Message { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(0);
            encoder.WriteBool(false);
            encoder.WriteString(Message);
            encoder.WriteString("");
        }
    }
}
