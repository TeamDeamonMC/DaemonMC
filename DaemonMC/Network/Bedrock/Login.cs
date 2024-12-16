namespace DaemonMC.Network.Bedrock
{
    public class Login
    {
        public Info.Bedrock id = Info.Bedrock.Login;

        public int protocolVersion = 0;
        public string request = "";


        public void Decode(PacketDecoder decoder)
        {
            var packet = new Login
            {
                protocolVersion = decoder.ReadIntBE(),
                request = decoder.ReadString(),
            };

            BedrockPacketProcessor.Login(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
