namespace DaemonMC.Network.Bedrock
{
    public class ClientCacheStatus
    {
        public Info.Bedrock id = Info.Bedrock.ClientCacheStatus;

        public bool status = false;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new ClientCacheStatus
            {
                status = decoder.ReadBool(),
            };

            BedrockPacketProcessor.ClientCacheStatus(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
