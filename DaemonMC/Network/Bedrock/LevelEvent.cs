using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class LevelEvent
    {
        public Info.Bedrock id = Info.Bedrock.LevelEvent;

        public LevelEvents EventID = LevelEvents.Undefined;
        public Vector3 Position = new Vector3();
        public int Data = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteSignedVarInt((int)EventID);
            encoder.WriteVec3(Position);
            encoder.WriteSignedVarInt(Data);
            encoder.handlePacket();
        }
    }
}
