namespace DaemonMC.Network.Bedrock
{
    public class LevelChunk : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.LevelChunk;

        public int chunkX = 0;
        public int chunkZ = 0;
        public int count = 0;
        public byte[] data = new byte[0];

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(chunkX);
            encoder.WriteSignedVarInt(chunkZ);
            encoder.WriteSignedVarInt(0);
            encoder.WriteVarInt(count);
            encoder.WriteBool(false);

            encoder.WriteVarInt(data.Count());
            foreach (var raw in data)
            {
                encoder.WriteByte(raw);
            }
        }
    }
}
