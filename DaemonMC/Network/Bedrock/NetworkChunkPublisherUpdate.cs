namespace DaemonMC.Network.Bedrock
{
    public class NetworkChunkPublisherUpdate : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.NetworkChunkPublisherUpdate;

        public int x = 0;
        public int y = 0;
        public int z = 0;
        public int radius = 0;


        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(x);
            encoder.WriteSignedVarInt(y);
            encoder.WriteSignedVarInt(z);

            encoder.WriteVarInt(radius * 16);

            encoder.WriteInt(0);
        }
    }
}
