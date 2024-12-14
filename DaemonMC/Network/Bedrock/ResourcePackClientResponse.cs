namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackClientResponsePacket
    {
        public byte response { get; set; }
    }

    public class ResourcePackClientResponse
    {
        public const int id = 8;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new ResourcePackClientResponsePacket
            {
                response = decoder.ReadByte(),
            };
            decoder.ReadShort();
            BedrockPacketProcessor.ResourcePackClientResponse(packet, decoder.endpoint);
        }

        public static void Encode(ResourcePackClientResponsePacket fields)
        {

        }
    }
}
