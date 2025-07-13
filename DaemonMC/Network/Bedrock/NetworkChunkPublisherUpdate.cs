namespace DaemonMC.Network.Bedrock
{
    public class NetworkChunkPublisherUpdate : Packet
    {
        public override int Id => (int) Info.Bedrock.NetworkChunkPublisherUpdate;

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public int Radius { get; set; } = 0;


        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(X);
            encoder.WriteSignedVarInt(Y);
            encoder.WriteSignedVarInt(Z);
            encoder.WriteVarInt(Radius * 16);
            encoder.WriteInt(0); //todo
        }
    }
}
