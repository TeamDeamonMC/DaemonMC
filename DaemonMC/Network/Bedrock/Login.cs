namespace DaemonMC.Network.Bedrock
{
    public class Login : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Login;

        public int protocolVersion = 0;
        public string request = "";


        protected override void Decode(PacketDecoder decoder)
        {
            protocolVersion = decoder.ReadIntBE();
            request = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
