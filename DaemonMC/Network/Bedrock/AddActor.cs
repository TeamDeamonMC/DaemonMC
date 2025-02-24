using System.Numerics;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class AddActor : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.AddActor;

        public long EntityId { get; set; } = 0;
        public string ActorType { get; set; } = "";
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Velocity { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public float YbodyRotation { get; set; } = 0;
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarLong(EntityId);
            encoder.WriteVarLong((ulong)EntityId);
            encoder.WriteString(ActorType);
            encoder.WriteVec3(Position);
            encoder.WriteVec3(Velocity);
            encoder.WriteVec2(Rotation);
            encoder.WriteFloat(YheadRotation);
            encoder.WriteFloat(YbodyRotation);
            encoder.WriteVarInt(0);//attributes todo
            encoder.WriteMetadata(Metadata);
            encoder.WriteVarInt(0);//synched properties todo
            encoder.WriteVarInt(0);//synched properties todo
            encoder.WriteVarInt(0);//actor links todo
        }
    }
}
