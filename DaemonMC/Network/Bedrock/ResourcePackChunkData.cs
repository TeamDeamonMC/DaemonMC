namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackChunkData : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackChunkData;

        public string PackName { get; set; } = "";
        public int Chunk { get; set; } = 0;
        public long Offset { get; set; } = 0;
        public byte[] Data { get; set; } = [];

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(PackName);
            encoder.WriteInt(Chunk);
            encoder.WriteLong(Offset);
            encoder.WriteBytes(Data);
        }
    }
}
