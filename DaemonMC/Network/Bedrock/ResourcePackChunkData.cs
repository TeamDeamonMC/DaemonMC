namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackChunkData
    {
        public Info.Bedrock id = Info.Bedrock.ResourcePackChunkData;

        public string PackName = "";
        public int Chunk = 0;
        public long Offset = 0;
        public byte[] Data = new byte[0];

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteString(PackName);
            encoder.WriteInt(Chunk);
            encoder.WriteLong(Offset);
            encoder.WriteVarInt(Data.Count());
            encoder.WriteBytes(Data);
            encoder.handlePacket();
        }
    }
}
