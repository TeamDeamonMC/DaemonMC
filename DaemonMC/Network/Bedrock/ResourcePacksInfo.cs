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

        public static void Encode(ResourcePacksInfoPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteBool(fields.force);
            encoder.WriteBool(fields.isAddon);
            encoder.WriteBool(fields.hasScripts);
            encoder.WriteUUID(fields.templateUUID);
            encoder.WriteString(fields.templateVersion);
            encoder.WriteShort(0);
            encoder.handlePacket();
        }
    }
}
