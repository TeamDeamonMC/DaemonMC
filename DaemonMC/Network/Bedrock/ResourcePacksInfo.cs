using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class ResourcePacksInfo
    {
        public Info.Bedrock id = Info.Bedrock.ResourcePacksInfo;

        public bool force = false;
        public bool isAddon = false;
        public bool hasScripts = false;
        public Guid templateUUID = new Guid();
        public string templateVersion = "";

        public void Decode(byte[] buffer)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteBool(force);
            encoder.WriteBool(isAddon);
            encoder.WriteBool(hasScripts);
            if (encoder.protocolVersion >= Info.v1_21_50)
            {
                encoder.WriteUUID(templateUUID);
                encoder.WriteString(templateVersion);
            }
            encoder.WriteShort(0);//texture packs
            encoder.handlePacket();
        }
    }
}
