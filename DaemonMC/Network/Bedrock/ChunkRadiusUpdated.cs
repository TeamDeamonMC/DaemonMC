namespace DaemonMC.Network.Bedrock
{
    public class ChunkRadiusUpdated
    {
        public Info.Bedrock id = Info.Bedrock.ChunkRadiusUpdated;

        public int radius = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarInt(radius);
            encoder.handlePacket();
        }
    }
}
