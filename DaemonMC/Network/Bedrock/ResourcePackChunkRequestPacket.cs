using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackChunkRequest : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackChunkRequest;

        public string PackName = "";
        public int Chunk = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            PackName = decoder.ReadString();
            Chunk = decoder.ReadInt();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
