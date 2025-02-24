using System.Numerics;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class AddPlayer : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.AddPlayer;

        public Guid UUID { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = "";
        public long EntityId { get; set; } = 0;
        public string PlatformChatId { get; set; } = "";
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Velocity { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public int GameMode { get; set; } = 0;
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public string DeviceId { get; set; } = "";
        public int BuildPlatform { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteUUID(UUID);
            encoder.WriteString(Username);
            encoder.WriteVarLong(EntityId);
            encoder.WriteString(PlatformChatId);
            encoder.WriteVec3(Position);
            encoder.WriteVec3(Velocity);
            encoder.WriteVec2(Rotation);
            encoder.WriteFloat(YheadRotation);

            //todo items
            encoder.WriteSignedVarInt(0);

            encoder.WriteVarInt(GameMode);
            encoder.WriteMetadata(Metadata);

            //todo synched properties
            encoder.WriteVarInt(0);
            encoder.WriteVarInt(0);

            //todo serialized abilities data
            encoder.WriteLong(EntityId);
            encoder.WriteByte(0);
            encoder.WriteByte(0);
            encoder.WriteByte(0);

            //todo actor links
            encoder.WriteVarInt(0);

            encoder.WriteString(DeviceId);
            encoder.WriteInt(BuildPlatform);
        }
    }
}
