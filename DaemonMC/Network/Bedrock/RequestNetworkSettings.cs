namespace DaemonMC.Network.Bedrock
{
    public class RequestNetworkSettings
    {
        public Info.Bedrock id = Info.Bedrock.RequestNetworkSettings;

        public int protocolVersion = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new RequestNetworkSettings
            {
                protocolVersion = decoder.ReadIntBE(),
            };

            BedrockPacketProcessor.RequestNetworkSettings(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
