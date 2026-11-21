namespace DaemonMC.Network.Bedrock
{
    public class LevelChunk : Packet
    {
        public override int Id => (int) Info.Bedrock.LevelChunk;

        public int ChunkX { get; set; } = 0;
        public int ChunkZ { get; set; } = 0;
        public int Dimension { get; set; } = 0;
        public int SubChunkLimit { get; set; } = 0;

        public byte[] Data { get; set; } = [];

        private const uint RequestModeLimited = uint.MaxValue - 1;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(ChunkX);
            encoder.WriteSignedVarInt(ChunkZ);
            encoder.WriteSignedVarInt(Dimension);
            if (encoder.protocolVersion >= Info.v1_26_40)
            {
                encoder.WriteVarInt(0); //sub chunk count
                encoder.WriteBool(true); // lient sub chunk request limit. request mode
                encoder.WriteSignedVarInt(SubChunkLimit);
                encoder.WriteBool(false); //cache enabled
                encoder.WriteVarInt(0); //cache blob ids
            }
            else
            {
                encoder.WriteVarInt(RequestModeLimited);
                encoder.WriteShort((ushort)SubChunkLimit);
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
