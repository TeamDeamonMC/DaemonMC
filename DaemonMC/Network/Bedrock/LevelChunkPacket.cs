namespace DaemonMC.Network.Bedrock
{
    public class LevelChunkPacket
    {
        public int chunkX { get; set; }
        public int chunkZ { get; set; }
        public int count { get; set; }
        public byte[] data { get; set; }
    }

    public class LevelChunk
    {
        public static int id = 58;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(LevelChunkPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteSignedVarInt(fields.chunkX);
            encoder.WriteSignedVarInt(fields.chunkZ);
            encoder.WriteSignedVarInt(0);
            encoder.WriteVarInt(fields.count);
            encoder.WriteBool(false);

            encoder.WriteVarInt(fields.data.Count());
            foreach (var raw in fields.data)
            {
                encoder.WriteByte(raw);
            }

            encoder.handlePacket();
        }
    }
}
