namespace DaemonMC.Network.Bedrock
{
    public class RequestChunkRadius
    {
        public Info.Bedrock id = Info.Bedrock.RequestChunkRadius;

        public int radius = 0;
        public byte maxRadius = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new RequestChunkRadius
            {
                radius = decoder.ReadVarInt(),
                maxRadius = decoder.ReadByte()
            };
            BedrockPacketProcessor.RequestChunkRadius(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
