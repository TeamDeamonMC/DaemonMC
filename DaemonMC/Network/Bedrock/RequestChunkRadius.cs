namespace DaemonMC.Network.Bedrock
{
    public class RequestChunkRadius : Packet
    {
        public override int Id => (int) Info.Bedrock.RequestChunkRadius;

        public int Radius { get; set; } = 0;
        public byte MaxRadius { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            Radius = decoder.ReadVarInt();
            MaxRadius = decoder.ReadByte();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
