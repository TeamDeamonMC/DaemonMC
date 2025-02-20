using DaemonMC.Utils;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerList : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PlayerList;

        public byte action = 0;
        public Guid UUID = new Guid();
        public long EntityId = 0;
        public string Username = "";
        public string XUID = "";
        public string PlatformChatId = "";
        public int BuildPlatform = 0;
        public Skin Skin = new Skin();
        public bool IsTeacher = false;
        public bool IsHost = false;
        public bool IsSubclient = false;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(action);
            if (action == 0)
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
