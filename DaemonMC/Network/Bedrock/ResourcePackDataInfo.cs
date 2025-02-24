namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackDataInfo : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackDataInfo;

        public string PackName { get; set; } = "";
        public int ChunkSize { get; set; } = 0;
        public int ChunkCount { get; set; } = 0;
        public long PackSize { get; set; } = 0;
        public byte[] Hash { get; set; } = [];
        public bool IsPremium { get; set; } = false;
        public byte PackType { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(PackName);
            encoder.WriteInt(ChunkSize);
            encoder.WriteInt(ChunkCount);
            encoder.WriteLong(PackSize);
            encoder.WriteBytes(Hash);
            encoder.WriteBool(IsPremium);
            encoder.WriteByte(PackType);
        }
    }
}
