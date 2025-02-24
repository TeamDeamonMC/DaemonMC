namespace DaemonMC.Network.Bedrock
{
    public class AnimateEntity : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.AnimateEntity;

        public string Animation { get; set; } = "";
        public string NextState { get; set; } = "";
        public string StopExpression { get; set; } = "";
        public int StopExpressionMolang { get; set; } = 0;
        public string Controller { get; set; } = "";
        public float BlendOutTime { get; set; } = 0;
        public long RuntimeId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(Animation);
            encoder.WriteString(NextState);
            encoder.WriteString(StopExpression);
            encoder.WriteInt(StopExpressionMolang);
            encoder.WriteString(Controller);
            encoder.WriteFloat(BlendOutTime);
            encoder.WriteVarInt(1);
            encoder.WriteVarLong(RuntimeId);
        }
    }
}
