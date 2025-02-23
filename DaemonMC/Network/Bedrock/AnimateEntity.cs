namespace DaemonMC.Network.Bedrock
{
    public class AnimateEntity : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.AnimateEntity;

        public string mAnimation = "";
        public string mNextState = "";
        public string mStopExpression = "";
        public int StopExpressionMolang = 0;
        public string mController = "";
        public float mBlendOutTime = 0;
        public long mRuntimeId = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(mAnimation);
            encoder.WriteString(mNextState);
            encoder.WriteString(mStopExpression);
            encoder.WriteInt(StopExpressionMolang);
            encoder.WriteString(mController);
            encoder.WriteFloat(mBlendOutTime);
            encoder.WriteVarInt(1);
            encoder.WriteVarLong((ulong)mRuntimeId);
        }
    }
}
