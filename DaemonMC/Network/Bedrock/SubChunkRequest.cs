namespace DaemonMC.Network.Bedrock
{
    public class SubChunkRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.SubChunkRequest;

        public int Dimension { get; set; } = 0;
        public int CenterX { get; set; } = 0;
        public int CenterY { get; set; } = 0;
        public int CenterZ { get; set; } = 0;
        public List<(sbyte x, sbyte y, sbyte z)> Offsets { get; set; } = new List<(sbyte x, sbyte y, sbyte z)>();

        protected override void Decode(PacketDecoder decoder)
        {
            Dimension = decoder.ReadSignedVarInt();

            if (decoder.protocolVersion >= Info.v1_26_30)
            {
                int count = decoder.ReadVarInt();
                Offsets = decoder.ReadOffsets(decoder, count);
                CenterX = decoder.ReadInt();
                CenterY = decoder.ReadInt();
                CenterZ = decoder.ReadInt();
            }
            else
            {
                CenterX = decoder.ReadSignedVarInt();
                CenterY = decoder.ReadSignedVarInt();
                CenterZ = decoder.ReadSignedVarInt();
                int count = decoder.ReadInt();
                Offsets = decoder.ReadOffsets(decoder, count);
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
