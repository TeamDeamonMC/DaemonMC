using System.Numerics;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class AddActor
    {
        public Info.Bedrock id = Info.Bedrock.AddActor;

        public long EntityId = 0;
        public string ActorType = "";
        public Vector3 Position = new Vector3();
        public Vector3 Velocity = new Vector3();
        public Vector2 Rotation = new Vector2();
        public float YheadRotation = 0;
        public float YbodyRotation = 0;
        public Dictionary<ActorData, Metadata> Metadata = new Dictionary<ActorData, Metadata>();

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteSignedVarLong(EntityId);
            encoder.WriteVarLong((ulong)EntityId);
            encoder.WriteString(ActorType);
            encoder.WriteVec3(Position);
            encoder.WriteVec3(Velocity);
            encoder.WriteVec2(Rotation);
            encoder.WriteFloat(YheadRotation);
            encoder.WriteFloat(YbodyRotation);
            encoder.WriteVarInt(0);
            encoder.WriteMetadata(Metadata);
            encoder.WriteVarInt(0);
            encoder.WriteVarInt(0);
            encoder.WriteVarInt(0);
            encoder.handlePacket();
        }
    }
}
