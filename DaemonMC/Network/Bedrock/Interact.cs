using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class Interact : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Interact;

        public byte action = 0;
        public long actorRuntimeId = 0;
        public Vector3 interactPosition = new Vector3();

        protected override void Decode(PacketDecoder decoder)
        {
            action = decoder.ReadByte();
            actorRuntimeId = decoder.ReadVarLong();
            if (action == 3 || action == 4)
            {
                interactPosition = decoder.ReadVec3();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
