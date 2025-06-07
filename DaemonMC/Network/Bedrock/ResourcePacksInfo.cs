namespace DaemonMC.Network.Bedrock
{
    public class ResourcePacksInfo : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePacksInfo;

        public bool Force { get; set; } = false;
        public bool IsAddon { get; set; } = false;
        public bool HasScripts { get; set; } = false;
        public bool DisableVibrantVisuals { get; set; } = false;
        public Guid TemplateUUID { get; set; } = new Guid();
        public string TemplateVersion { get; set; } = "";
        public List<ResourcePack> Packs { get; set; } = new List<ResourcePack>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBool(Force);
            encoder.WriteBool(IsAddon);
            encoder.WriteBool(HasScripts);
            if (encoder.protocolVersion >= Info.v1_21_90)
            {
                encoder.WriteBool(DisableVibrantVisuals);
            }
            if (encoder.protocolVersion >= Info.v1_21_50)
            {
                encoder.WriteUUID(TemplateUUID);
                encoder.WriteString(TemplateVersion);
            }
            encoder.WriteResourcePacksInfo(Packs);
        }
    }
}
