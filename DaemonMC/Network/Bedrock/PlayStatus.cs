namespace DaemonMC.Network.Bedrock
{
    public class PlayStatus : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PlayStatus;

        public int Status { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteIntBE(Status);
        }
    }
}
