namespace DaemonMC.Network.Bedrock
{
    public class LoginPacket
    {
        public int protocolVersion { get; set; }
        public string request { get; set; }
    }

    public class Login
    {
        public const int id = 1;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new LoginPacket
            {
                protocolVersion = decoder.ReadIntBE(),
                request = decoder.ReadString(),
            };

            BedrockPacketProcessor.Login(packet, decoder.endpoint);
        }

        public static void Encode(LoginPacket fields)
        {

        }
    }
}
