namespace DaemonMC.Network.Bedrock
{
    public class Login : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Login;

        public int ProtocolVersion { get; set; } = 0;
        public string Request { get; set; } = "";


        protected override void Decode(PacketDecoder decoder)
        {
            ProtocolVersion = decoder.ReadIntBE();
            Request = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
