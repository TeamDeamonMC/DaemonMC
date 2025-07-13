namespace DaemonMC.Network.Bedrock
{
    public class Animate : Packet
    {
        public override int Id => (int) Info.Bedrock.Animate;

        public int Action { get; set; } = 0;
        public long EntityId { get; set; } = 0;
        public float RowingTime { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            Action = decoder.ReadVarInt();
            EntityId = decoder.ReadVarLong();
            if (Action == 128 || Action == 129)
            {
                RowingTime = decoder.ReadFloat();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(Action);
            encoder.WriteVarLong(EntityId);
            if (Action == 128 || Action == 129)
            {
                encoder.WriteFloat(RowingTime);
            }
        }
    }
}
