namespace DaemonMC.Network.Bedrock
{
    public class TextMessage : Packet
    {
        public override int Id => (int) Info.Bedrock.TextMessage;

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
            if (MessageType == 0 || MessageType == 5 || MessageType == 6 || MessageType == 9 || MessageType == 10 || MessageType == 11)
            {
                encoder.WriteString(Message);
            }
            if (MessageType == 1 || MessageType == 7 || MessageType == 8)
            {
                encoder.WriteString(Username);
                encoder.WriteString(Message);
            }
            if (MessageType == 2 || MessageType == 3 || MessageType == 4)
            {
                encoder.WriteString(Message);
                encoder.WriteVarInt(0);
            }
            encoder.WriteString(XUID);
            encoder.WriteString(PlatformId);
            encoder.WriteString(FilteredMessage);
        }
    }
}
