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
        public List<ResourcePack> packs = new List<ResourcePack>();

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
            encoder.WriteShort((ushort) packs.Count());
            foreach (var pack in packs)
            {
                encoder.WriteUUID(pack.UUID);
                encoder.WriteString(pack.PackIdVersion);
                encoder.WriteLong(pack.PackContent.Length);
                encoder.WriteString(pack.ContentKey);
                encoder.WriteString(pack.SubpackName);
                encoder.WriteString(pack.ContentId);
                encoder.WriteBool(pack.HasScripts);
                encoder.WriteBool(pack.IsAddon);
                encoder.WriteBool(pack.RayTracking);
                encoder.WriteString(pack.CdnUrl);
            }
            encoder.handlePacket();
        }
    }
}
