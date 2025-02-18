namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackClientResponse
    {
        public Info.Bedrock id = Info.Bedrock.ResourcePackClientResponse;

        public byte response = 0;
        public List<string> packs = new List<string>();

        public void Decode(PacketDecoder decoder)
        {
            var packet = new ResourcePackClientResponse
            {
                response = decoder.ReadByte(),
                packs = decoder.ReadPackNames()
            };
            BedrockPacketProcessor.ResourcePackClientResponse(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
