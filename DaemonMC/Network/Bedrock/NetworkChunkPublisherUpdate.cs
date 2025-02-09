namespace DaemonMC.Network.Bedrock
{
    public class NetworkChunkPublisherUpdate
    {
        public Info.Bedrock id = Info.Bedrock.NetworkChunkPublisherUpdate;

        public int x = 0;
        public int y = 0;
        public int z = 0;
        public int radius = 0;


        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);

            encoder.WriteSignedVarInt(x);
            encoder.WriteSignedVarInt(y);
            encoder.WriteSignedVarInt(z);

            encoder.WriteVarInt(radius * 16);

            encoder.WriteInt(0);

            encoder.handlePacket();
        }
    }
}
