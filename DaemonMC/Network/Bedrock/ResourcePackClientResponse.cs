using DaemonMC.Network.Enumerations;

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
            if (decoder.protocolVersion >= Info.v1_26_40)
            {
                Response = (byte)(decoder.ReadVarInt() + 1);
                ResponseType = decoder.ReadString();
                if (Response == (byte)ResourcePackClientResponseStatus.SendPacks)
                {
                    Packs = decoder.ReadPackNames();
                }
            }
            else
            {
                Response = decoder.ReadByte();
                Packs = decoder.ReadPackNames();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (encoder.protocolVersion >= Info.v1_26_40)
            {
                encoder.WriteVarInt(Response - 1);
                encoder.WriteString(string.IsNullOrEmpty(ResponseType) ? ResourcePackClientResponseStatusNames.Values[Math.Clamp(Response - 1, 0, ResourcePackClientResponseStatusNames.Values.Length - 1)] : ResponseType);
                if (Response == (byte)ResourcePackClientResponseStatus.SendPacks)
                {
                    encoder.WritePackNames(Packs);
                }
            }
            else
            {
                encoder.WriteByte(Response);
                encoder.WritePackNames(Packs);
            }
        }
    }
}
