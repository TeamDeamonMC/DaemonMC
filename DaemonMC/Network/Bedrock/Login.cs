namespace DaemonMC.Network.Bedrock
{
    public class Login : Packet
    {
        public override int Id => (int) Info.Bedrock.Login;

        public int ProtocolVersion { get; set; } = 0;
        public byte[] Request { get; set; }


        protected override void Decode(PacketDecoder decoder)
        {
            ProtocolVersion = decoder.ReadIntBE();
            Request = decoder.ReadBytes();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteIntBE(ProtocolVersion);
            encoder.WriteBytes(Request, true);
        }
    }
}
