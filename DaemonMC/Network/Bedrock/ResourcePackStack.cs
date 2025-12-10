namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackStack : Packet
    {
        public override int Id => (int) Info.Bedrock.ResourcePackStack;

        public bool ForceTexturePack { get; set; } = false;
        public List<ResourcePack> Packs { get; set; } = new List<ResourcePack>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBool(ForceTexturePack);
            if (encoder.protocolVersion < Info.v1_21_130)
            {
                encoder.WriteVarInt(0); //add-on list
            }
            encoder.WriteResourcePacksStack(Packs);
            encoder.WriteString(Info.Version);
            encoder.WriteInt(0); //experiments
            encoder.WriteBool(false); //experiments was on
            encoder.WriteBool(false); //editor packs
        }
    }
}
