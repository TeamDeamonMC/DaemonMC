namespace DaemonMC.Network.Bedrock
{
    public class Animate : Packet
    {
        public override int Id => (int) Info.Bedrock.Animate;

        public int Action { get; set; } = 0;
        public long EntityId { get; set; } = 0;
        public float Data { get; set; } = 0;
        public float RowingTime { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            Action = decoder.ReadVarInt();
            EntityId = decoder.ReadVarLong();
            if (decoder.protocolVersion >= Info.v1_21_120)
            {
                Data = decoder.ReadFloat();
            }
            if (Action == 128 || Action == 129)
            {
                RowingTime = decoder.ReadFloat();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(Action);
            encoder.WriteVarLong(EntityId);
            if (encoder.protocolVersion >= Info.v1_21_120)
            {
                encoder.WriteFloat(Data);
            }
            if (Action == 128 || Action == 129)
            {
                encoder.WriteFloat(RowingTime);
            }
        }
    }
}
