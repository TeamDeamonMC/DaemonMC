namespace DaemonMC.Network.Bedrock
{
    public class Animate : Packet
    {
        public override int Id => (int) Info.Bedrock.Animate;

        public int Action { get; set; } = 0;
        public long EntityId { get; set; } = 0;
        public float Data { get; set; } = 0;
        public float RowingTime { get; set; } = 0;
        public string? SwingSource { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {
            Action = decoder.ReadVarInt(); //todo what after v1_21_130?
            EntityId = decoder.ReadVarLong();
            if (decoder.protocolVersion >= Info.v1_21_120)
            {
                Data = decoder.ReadFloat();
            }
            if (Action == 128 || Action == 129)
            {
                RowingTime = decoder.ReadFloat();
            }
            if (decoder.protocolVersion >= Info.v1_21_130)
            {
                SwingSource = decoder.ReadOptional(decoder.ReadString);
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
            if (encoder.protocolVersion >= Info.v1_21_130)
            {
                encoder.WriteOptional(string.IsNullOrEmpty(SwingSource) ? null : () => encoder.WriteString(SwingSource!));
            }
        }
    }
}
