namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackClientResponse : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackClientResponse;

        public byte Response { get; set; } = 0;
        public List<string> Packs { get; set; } = new List<string>();

        protected override void Decode(PacketDecoder decoder)
        {
            Response = decoder.ReadByte();
            Packs = decoder.ReadPackNames();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
