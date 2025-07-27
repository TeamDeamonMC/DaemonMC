namespace DaemonMC.Network.Bedrock
{
    public class PlayStatus : Packet
    {
        public override int Id => (int) Info.Bedrock.PlayStatus;

        public int Status { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            Status = decoder.ReadIntBE();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteIntBE(Status);
        }
    }
}
