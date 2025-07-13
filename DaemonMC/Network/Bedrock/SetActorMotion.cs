using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class SetActorMotion : Packet
    {
        public override int Id => (int) Info.Bedrock.SetActorMotion;

        public long EntityId { get; set; } = 0;
        public Vector3 Motion { get; set; } = new Vector3();
        public long Tick { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteVec3(Motion);
            encoder.WriteVarLong(Tick);
        }
    }
}
