namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackClientResponse : Packet
    {
        public override int Id => (int) Info.Bedrock.ResourcePackClientResponse;

        public byte Response { get; set; } = 0;
        public string ResponseType { get; set; } = "";
        public List<string> Packs { get; set; } = new List<string>();

        protected override void Decode(PacketDecoder decoder)
        {
            Response = (byte)(decoder.ReadByte() + 1); //sigh
            if (decoder.protocolVersion >= Info.v1_26_40)
            {
                ResponseType = decoder.ReadString();
                if (Response == 1)
                {
                    Packs = decoder.ReadPackNames();
                }
            }
            else
            {
                Packs = decoder.ReadPackNames();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte((byte)(Response - 1));
            if (encoder.protocolVersion >= Info.v1_26_40)
            {
                encoder.WriteString(ResponseType);
                if (Response == Response)
                {
                    encoder.WritePackNames(Packs);
                }
            }
            else
            {
                encoder.WritePackNames(Packs);
            }
        }
    }
}
