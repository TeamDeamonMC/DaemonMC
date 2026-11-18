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
        public byte Hand { get; set; } = 0; //0 main hand, 1 off hand

        protected override void Decode(PacketDecoder decoder)
        {
            bool v1_21_130 = decoder.protocolVersion >= Info.v1_21_130;
            Action = v1_21_130 ? decoder.ReadByte() : decoder.ReadSignedVarInt();
            EntityId = decoder.ReadVarLong();
            if (decoder.protocolVersion >= Info.v1_21_120)
            {
                Data = decoder.ReadFloat();
            }
            if (!v1_21_130 && (Action == 128 || Action == 129))
            {
                RowingTime = decoder.ReadFloat();
            }
            if (v1_21_130)
            {
                SwingSource = decoder.ReadOptional(decoder.ReadString);
            }
            if (decoder.protocolVersion >= Info.v1_26_60)
            {
                Hand = decoder.ReadByte();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            bool v1_21_130 = encoder.protocolVersion >= Info.v1_21_130;
            if (v1_21_130)
            {
                encoder.WriteByte((byte)Action);
            }
            else
            {
                encoder.WriteSignedVarInt(Action);
            }
            encoder.WriteVarLong(EntityId);
            if (encoder.protocolVersion >= Info.v1_21_120)
            {
                encoder.WriteFloat(Data);
            }
            if (!v1_21_130 && (Action == 128 || Action == 129))
            {
                encoder.WriteFloat(RowingTime);
            }
            if (v1_21_130)
            {
                encoder.WriteOptional(string.IsNullOrEmpty(SwingSource) ? null : () => encoder.WriteString(SwingSource!));
            }
            if (encoder.protocolVersion >= Info.v1_26_60)
            {
                encoder.WriteByte(Hand);
            }
        }
    }
}
