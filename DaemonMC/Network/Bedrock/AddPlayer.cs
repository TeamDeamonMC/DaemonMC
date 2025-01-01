using System.Numerics;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class AddPlayer
    {
        public Info.Bedrock id = Info.Bedrock.AddPlayer;

        public Guid UUID = Guid.NewGuid();
        public string Username = "";
        public long EntityId = 0;
        public string PlatformChatId = "";
        public Vector3 Position = new Vector3();
        public Vector3 Velocity = new Vector3();
        public Vector2 Rotation = new Vector2();
        public float YheadRotation = 0;
        public int GameMode = 0;
        public Dictionary<ActorData, Metadata> Metadata = new Dictionary<ActorData, Metadata>();
        public string DeviceId = "";
        public int BuildPlatform = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteUUID(UUID);
            encoder.WriteString(Username);
            encoder.WriteVarLong((ulong)EntityId);
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
            encoder.handlePacket();
        }
    }
}
