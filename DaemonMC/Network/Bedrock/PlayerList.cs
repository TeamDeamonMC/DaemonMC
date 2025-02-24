using DaemonMC.Utils;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerList : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PlayerList;

        public byte Action { get; set; } = 0;
        public Guid UUID { get; set; } = new Guid();
        public long EntityId { get; set; } = 0;
        public string Username { get; set; } = "";
        public string XUID { get; set; } = "";
        public string PlatformChatId { get; set; } = "";
        public int BuildPlatform { get; set; } = 0;
        public Skin Skin { get; set; } = new Skin();
        public bool IsTeacher { get; set; } = false;
        public bool IsHost { get; set; } = false;
        public bool IsSubclient { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(Action);
            if (Action == 0)
            {
                encoder.WriteVarInt(1);
                encoder.WriteUUID(UUID);
                encoder.WriteSignedVarLong(EntityId);
                encoder.WriteString(Username);
                encoder.WriteString(XUID);
                encoder.WriteString(PlatformChatId);
                encoder.WriteInt(BuildPlatform);
                encoder.WriteSkin(Skin);
                encoder.WriteBool(IsTeacher);
                encoder.WriteBool(IsHost);
                encoder.WriteBool(IsSubclient);
                encoder.WriteBool(true);
            }
            else
            {
                encoder.WriteVarInt(1);
                encoder.WriteUUID(UUID);
            }
        }
    }
}
