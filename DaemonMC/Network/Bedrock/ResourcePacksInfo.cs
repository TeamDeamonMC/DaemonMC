namespace DaemonMC.Network.Bedrock
{
    public class ResourcePacksInfoPacket
    {
        public bool force { get; set; }
        public bool isAddon { get; set; }
        public bool hasScripts { get; set; }
        public Guid templateUUID { get; set; }
        public string templateVersion { get; set; } = "";
        //behaviour packs todo
        //resource packs todo
    }

    public class ResourcePacksInfo
    {
        public static int id = 6;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(ResourcePacksInfoPacket fields)
        {
            DataTypes.WriteVarInt(id);
            DataTypes.WriteBool(fields.force);
            DataTypes.WriteBool(fields.isAddon);
            DataTypes.WriteBool(fields.hasScripts);
            DataTypes.WriteUUID(fields.templateUUID);
            DataTypes.WriteString(fields.templateVersion);
            DataTypes.WriteShort(0);
            PacketEncoder.handlePacket();
        }
    }
}
