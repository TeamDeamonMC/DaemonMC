using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class LevelEvent : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.LevelEvent;

        public LevelEvents EventID = LevelEvents.Undefined;
        public Vector3 Position = new Vector3();
        public int Data = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt((int)EventID);
            encoder.WriteVec3(Position);
            encoder.WriteSignedVarInt(Data);
        }
    }
}
