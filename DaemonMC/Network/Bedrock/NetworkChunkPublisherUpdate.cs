namespace DaemonMC.Network.Bedrock
{
    public class NetworkChunkPublisherUpdatePacket
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int radius { get; set; }
    }

    public class NetworkChunkPublisherUpdate
    {
        public static int id = 121;
        public static void Decode(PacketDecoder decoder)
        {

        }

        public static void Encode(NetworkChunkPublisherUpdatePacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);

            encoder.WriteSignedVarInt(fields.x);
            encoder.WriteVarInt(fields.y);
            encoder.WriteSignedVarInt(fields.z);

            encoder.WriteVarInt(fields.radius * 16);

            encoder.WriteInt(0);

            encoder.handlePacket();
        }
    }
}
