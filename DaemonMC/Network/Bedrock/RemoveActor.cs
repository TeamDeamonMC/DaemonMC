namespace DaemonMC.Network.Bedrock
{
    public class RemoveActor : Packet
    {
        public override int Id => (int) Info.Bedrock.RemoveActor;

        public long EntityId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarLong(EntityId);
        }
    }
}
