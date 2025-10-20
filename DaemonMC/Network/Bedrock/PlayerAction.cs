namespace DaemonMC.Network.Bedrock
{
    public class PlayerAction : Packet
    {
        public override int Id => (int) Info.Bedrock.PlayerAction;

        public long EntityId { get; set; } = 0;
        public int Action { get; set; } = 0;
        public int BlockX { get; set; } = 0;
        public int BlockY { get; set; } = 0;
        public int BlockZ { get; set; } = 0;
        public int ResultX { get; set; } = 0;
        public int ResultY { get; set; } = 0;
        public int ResultZ { get; set; } = 0;
        public int Face { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            EntityId = decoder.ReadVarLong();
            Action = decoder.ReadSignedVarInt();
            BlockX = decoder.ReadSignedVarInt();
            BlockY = decoder.ReadVarInt();
            BlockZ = decoder.ReadSignedVarInt();
            ResultX = decoder.ReadSignedVarInt();
            ResultY = decoder.ReadVarInt();
            ResultZ = decoder.ReadSignedVarInt();
            Face = decoder.ReadSignedVarInt();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteVarInt(Action);
            encoder.WriteVarInt(BlockX);
            encoder.WriteSignedVarInt(BlockY);
            encoder.WriteVarInt(BlockZ);
            encoder.WriteVarInt(ResultX);
            encoder.WriteSignedVarInt(ResultY);
            encoder.WriteVarInt(ResultZ);
            encoder.WriteVarInt(Face);
        }
    }
}
