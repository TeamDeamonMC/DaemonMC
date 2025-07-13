using System.Numerics;
using DaemonMC.Entities;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class AddActor : Packet
    {
        public override int Id => (int) Info.Bedrock.AddActor;

        public long EntityId { get; set; } = 0;
        public string ActorType { get; set; } = "";
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Velocity { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public float YbodyRotation { get; set; } = 0;
        public List<AttributeValue> Attributes { get; set; } = new List<AttributeValue>();
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public SynchedProperties Properties { get; set; } = new SynchedProperties();
        public List<EntityLink> LinkedActors { get; set; } = new List<EntityLink>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarLong(EntityId);
            encoder.WriteVarLong(EntityId);
            encoder.WriteString(ActorType);
            encoder.WriteVec3(Position);
            encoder.WriteVec3(Velocity);
            encoder.WriteVec2(Rotation);
            encoder.WriteFloat(YheadRotation);
            encoder.WriteFloat(YbodyRotation);
            encoder.WriteActorAttributes(Attributes);
            encoder.WriteMetadata(Metadata);
            encoder.WriteProperties(Properties);
            encoder.WriteActorLinks(LinkedActors);
        }
    }
}
