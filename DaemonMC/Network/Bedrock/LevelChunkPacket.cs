using DaemonMC.Utils;
namespace DaemonMC.Network.Bedrock
{
    public class LevelChunkPacket
    {
        public int chunkX { get; set; }
        public int chunkZ { get; set; }
        public string data { get; set; }
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
            encoder.WriteVarInt(0);
            encoder.WriteBool(false);
            encoder.WriteString(fields.data);
            encoder.handlePacket();
        }
    }
}
