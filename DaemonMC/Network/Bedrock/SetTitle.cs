namespace DaemonMC.Network.Bedrock
{
    public class SetTitle : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.SetTitle;

        public int Type { get; set; } = 0;
        public string Text { get; set; } = "";
        public int FadeIn { get; set; } = 20;
        public int Stay { get; set; } = 20;
        public int FadeOut { get; set; } = 20;
        public string XUID { get; set; } = "";
        public string PlatformID { get; set; } = "";
        public string FilteredMessage { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(Type);
            encoder.WriteString(Text);
            encoder.WriteSignedVarInt(FadeIn);
            encoder.WriteSignedVarInt(Stay);
            encoder.WriteSignedVarInt(FadeOut);
            encoder.WriteString(XUID);
            encoder.WriteString(PlatformID);
            encoder.WriteString(FilteredMessage);
        }
    }
}
