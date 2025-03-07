namespace DaemonMC.Network.Bedrock
{
    public class SetTime : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.SetTime;

        public int Time { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(Time);
        }
    }
}
