namespace DaemonMC.Network.Bedrock
{
    public class RequestNetworkSettingsPacket
    {
        public int protocolVersion { get; set; }
    }

    public class RequestNetworkSettings
    {
        public const int id = 193;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new RequestNetworkSettingsPacket
            {
                protocolVersion = decoder.ReadIntBE(),
            };

            BedrockPacketProcessor.RequestNetworkSettings(packet, decoder.endpoint);
        }

        public static void Encode(RequestNetworkSettingsPacket fields)
        {

        }
    }
}
