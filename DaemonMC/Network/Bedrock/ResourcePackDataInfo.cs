using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackDataInfo
    {
        public Info.Bedrock id = Info.Bedrock.ResourcePackDataInfo;

        public string PackName = "";
        public int ChunkSize = 0;
        public int ChunkCount = 0;
        public long PackSize = 0;
        public byte[] Hash = new byte[0];
        public bool IsPremium = false;
        public byte PackType = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteString(PackName);
            encoder.WriteInt(ChunkSize);
            encoder.WriteInt(ChunkCount);
            encoder.WriteLong(PackSize);
            encoder.WriteVarInt(Hash.Count());
            encoder.WriteBytes(Hash);
            encoder.WriteBool(IsPremium);
            encoder.WriteByte(PackType);
            encoder.handlePacket();
        }
    }
}
