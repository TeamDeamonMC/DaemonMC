using DaemonMC.Utils;
namespace DaemonMC.Network.Bedrock
{
    public class ChunkRadiusUpdatedPacket
    {
        public int radius { get; set; }
    }

    public class ChunkRadiusUpdated
    {
        public static int id = 70;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(ChunkRadiusUpdatedPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteVarInt(fields.radius);
            encoder.handlePacket();
        }
    }
}
