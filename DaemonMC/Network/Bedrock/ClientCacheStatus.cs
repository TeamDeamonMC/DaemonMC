namespace DaemonMC.Network.Bedrock
{
    public class ClientCacheStatus : Packet
    {
        public override int Id => (int) Info.Bedrock.ClientCacheStatus;

        public bool Status { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {
            Status = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBool(Status);
        }
    }
}
