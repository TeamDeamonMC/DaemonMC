using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class SetActorMotion : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.SetActorMotion;

        public long EntityId = 0;
        public Vector3 Motion = new Vector3();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong((ulong)EntityId);
            encoder.WriteVarLong(0);
        }
    }
}
