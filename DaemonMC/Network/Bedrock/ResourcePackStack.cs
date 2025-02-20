namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackStack : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackStack;

        public bool forceTexturePack = false;
        public List<ResourcePack> packs = new List<ResourcePack>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBool(forceTexturePack);
            encoder.WriteVarInt(0); //add-on list
            encoder.WriteVarInt(packs.Count); //texture pack list
            foreach (var pack in packs)
            {
                encoder.WriteString(pack.UUID.ToString());
                encoder.WriteString(pack.PackIdVersion);
                encoder.WriteString(pack.SubpackName);
            }
            encoder.WriteString(Info.version);
            encoder.WriteInt(0); //experiments
            encoder.WriteBool(false); //experiments was on
            encoder.WriteBool(false); //editor packs
        }
    }
}
