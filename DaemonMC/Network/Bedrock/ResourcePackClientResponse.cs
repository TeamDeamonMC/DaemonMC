namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackClientResponse : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ResourcePackClientResponse;

        public byte response = 0;
        public List<string> packs = new List<string>();

        protected override void Decode(PacketDecoder decoder)
        {
            response = decoder.ReadByte();
            packs = decoder.ReadPackNames();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
