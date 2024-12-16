namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackClientResponse
    {
        public Info.Bedrock id = Info.Bedrock.ResourcePackClientResponse;

        public byte response = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new ResourcePackClientResponse
            {
                response = decoder.ReadByte(),
            };
            decoder.ReadShort();
            BedrockPacketProcessor.ResourcePackClientResponse(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
