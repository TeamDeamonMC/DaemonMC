using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class SetActorMotion
    {
        public Info.Bedrock id = Info.Bedrock.SetActorMotion;

        public long EntityId = 0;
        public Vector3 Motion = new Vector3();

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong((ulong)EntityId);
            encoder.WriteVarLong(0);
            encoder.handlePacket();
        }
    }
}
