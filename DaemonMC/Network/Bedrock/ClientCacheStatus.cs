namespace DaemonMC.Network.Bedrock
{
    public class ClientCacheStatusPacket
    {
        public bool status { get; set; }
    }

    public class ClientCacheStatus
    {
        public const int id = 129;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new ClientCacheStatusPacket
            {
                status = decoder.ReadBool(),
            };

            BedrockPacketProcessor.ClientCacheStatus(packet, decoder.endpoint);
        }

        public static void Encode(ClientCacheStatusPacket fields)
        {

        }
    }
}
