namespace DaemonMC.Network.Bedrock
{
    public class LevelChunk : Packet
    {
        public override int Id => (int) Info.Bedrock.LevelChunk;

        public int ChunkX { get; set; } = 0;
        public int ChunkZ { get; set; } = 0;
        public int Dimension { get; set; } = 0;
        public int Count { get; set; } = 0;
        public byte[] Data { get; set; } = [];

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(ChunkX);
            encoder.WriteSignedVarInt(ChunkZ);
            encoder.WriteSignedVarInt(Dimension);
            encoder.WriteVarInt(Count);
            if (encoder.protocolVersion >= Info.v1_26_40)
            {
                encoder.WriteBool(false); //optional client sub chunk request limit
                encoder.WriteBool(false); //cache enabled
                encoder.WriteVarInt(0); //cache blob ids
            }
            else
            {
                encoder.WriteBool(false); //cache enabled
            }
            encoder.WriteBytes(Data);
            if (encoder.protocolVersion >= Info.v1_26_60)
            {
                encoder.WriteBool(false); //client biome update
            }
        }
    }
}
