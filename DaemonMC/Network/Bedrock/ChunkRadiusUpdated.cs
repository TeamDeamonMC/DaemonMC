namespace DaemonMC.Network.Bedrock
{
    public class ChunkRadiusUpdated : Packet
    {
        public override int Id => (int) Info.Bedrock.ChunkRadiusUpdated;

        public int Radius { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(Radius);
        }
    }
}
