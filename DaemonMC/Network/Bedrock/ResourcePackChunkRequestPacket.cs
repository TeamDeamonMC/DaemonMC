using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackChunkRequest
    {
        public Info.Bedrock id = Info.Bedrock.ResourcePackChunkRequest;

        public string PackName = "";
        public int Chunk = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new ResourcePackChunkRequest
            {
                PackName = decoder.ReadString(),
                Chunk = decoder.ReadInt()
            };

            BedrockPacketProcessor.ResourcePackChunkRequest(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
