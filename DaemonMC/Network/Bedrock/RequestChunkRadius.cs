namespace DaemonMC.Network.Bedrock
{
    public class RequestChunkRadiusPacket
    {
        public int radius { get; set; }
        public byte maxRadius { get; set; }
    }

    public class RequestChunkRadius
    {
        public const int id = 69;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new RequestChunkRadiusPacket
            {
                radius = decoder.ReadVarInt(),
                maxRadius = decoder.ReadByte()
            };
            BedrockPacketProcessor.RequestChunkRadius(packet, decoder.endpoint);
        }

        public static void Encode(RequestChunkRadiusPacket fields)
        {

        }
    }
}
