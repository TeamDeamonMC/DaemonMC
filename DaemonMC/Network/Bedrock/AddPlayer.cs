using System.Numerics;
using DaemonMC.Entities;
using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;
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
        public Item Item { get; set; } = new Air();
        public float YheadRotation { get; set; } = 0;
        public int GameMode { get; set; } = 0;
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public SynchedProperties Properties { get; set; } = new SynchedProperties();
        public byte PlayerPermissions { get; set; } = 0;
        public byte CommandPermissions { get; set; } = 0;
        public List<AbilitiesData> Layers { get; set; } = new List<AbilitiesData>();
        public List<EntityLink> LinkedActors { get; set; } = new List<EntityLink>();
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
            encoder.WriteItem(Item);
            encoder.WriteVarInt(GameMode);
            encoder.WriteMetadata(Metadata);
            encoder.WriteProperties(Properties);
            encoder.WriteLong(EntityId);
            encoder.WriteByte(PlayerPermissions);
            encoder.WriteByte(CommandPermissions);
            encoder.WriteAbilitiesData(Layers);
            encoder.WriteActorLinks(LinkedActors);
            encoder.WriteString(DeviceId);
            encoder.WriteInt(BuildPlatform);
        }
    }
}
