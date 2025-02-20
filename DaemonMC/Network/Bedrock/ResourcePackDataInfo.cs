using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackDataInfo : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackDataInfo;

        public string PackName = "";
        public int ChunkSize = 0;
        public int ChunkCount = 0;
        public long PackSize = 0;
        public byte[] Hash = new byte[0];
        public bool IsPremium = false;
        public byte PackType = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(PackName);
            encoder.WriteInt(ChunkSize);
            encoder.WriteInt(ChunkCount);
            encoder.WriteLong(PackSize);
            encoder.WriteVarInt(Hash.Count());
            encoder.WriteBytes(Hash);
            encoder.WriteBool(IsPremium);
            encoder.WriteByte(PackType);
        }
    }
}
