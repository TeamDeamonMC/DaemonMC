namespace DaemonMC.Network.Bedrock
{
    public class TextMessage : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.TextMessage;

        public byte MessageType { get; set; } = 0;
        public bool Localized { get; set; } = false;
        public string Username { get; set; } = "";
        public string Message { get; set; } = "";
        public string XUID { get; set; } = "";
        public string PlatformId { get; set; } = "";
        public string FilteredMessage { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {
            MessageType = decoder.ReadByte();
            Localized = decoder.ReadBool();
            Username = decoder.ReadString();
            Message = decoder.ReadString();
            XUID = decoder.ReadString();
            PlatformId = decoder.ReadString();
            FilteredMessage = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(MessageType);
            encoder.WriteBool(Localized);
            encoder.WriteString(Username);
            encoder.WriteString(Message);
            encoder.WriteString(XUID);
            encoder.WriteString(PlatformId);
            encoder.WriteString(FilteredMessage);
        }
    }
}
