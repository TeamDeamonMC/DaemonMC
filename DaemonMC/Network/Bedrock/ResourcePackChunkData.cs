namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackChunkData : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackChunkData;

        public string PackName = "";
        public int Chunk = 0;
        public long Offset = 0;
        public byte[] Data = new byte[0];

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(PackName);
            encoder.WriteInt(Chunk);
            encoder.WriteLong(Offset);
            encoder.WriteVarInt(Data.Count());
            encoder.WriteBytes(Data);
        }
    }
}
