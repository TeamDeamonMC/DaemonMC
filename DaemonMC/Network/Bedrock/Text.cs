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
            if (decoder.protocolVersion >= Info.v1_21_130)
            {
                Localized = decoder.ReadBool();
                byte category = decoder.ReadByte();
                switch (category)
                {
                    case 0:
                        decoder.ReadString(); //todo fix
                        decoder.ReadString();
                        decoder.ReadString();
                        decoder.ReadString();
                        decoder.ReadString();
                        decoder.ReadString();
                        break;
                    case 1:
                        decoder.ReadString();
                        decoder.ReadString();
                        decoder.ReadString();
                        break;
                    case 2:
                        decoder.ReadString();
                        decoder.ReadString();
                        decoder.ReadString();
                        break;
                }
                MessageType = decoder.ReadByte();
            }
            else
            {
                MessageType = decoder.ReadByte();
                Localized = decoder.ReadBool();
            }
            Username = decoder.ReadString();
            Message = decoder.ReadString();
            XUID = decoder.ReadString();
            PlatformId = decoder.ReadString();
            FilteredMessage = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (encoder.protocolVersion < Info.v1_21_130)
            {
                encoder.WriteByte(MessageType);
            }
            encoder.WriteBool(Localized);
            if (MessageType == 0 || MessageType == 5 || MessageType == 6 || MessageType == 9 || MessageType == 10 || MessageType == 11)
            {
                if (encoder.protocolVersion >= Info.v1_21_130)
                {
                    encoder.WriteByte(0);
                    encoder.WriteString("raw");
                    encoder.WriteString("tip");
                    encoder.WriteString("systemMessage");
                    encoder.WriteString("textObjectWhisper");
                    encoder.WriteString("textObjectAnnouncement");
                    encoder.WriteString("textObject");
                    encoder.WriteByte(MessageType);
                }
                encoder.WriteString(Message);
            }
            if (MessageType == 1 || MessageType == 7 || MessageType == 8)
            {
                if (encoder.protocolVersion >= Info.v1_21_130)
                {
                    encoder.WriteByte(1);
                    encoder.WriteString("chat");
                    encoder.WriteString("whisper");
                    encoder.WriteString("announcement");
                    encoder.WriteByte(MessageType);
                }
                encoder.WriteString(Username);
                encoder.WriteString(Message);
            }
            if (MessageType == 2 || MessageType == 3 || MessageType == 4)
            {
                if (encoder.protocolVersion >= Info.v1_21_130)
                {
                    encoder.WriteByte(2);
                    encoder.WriteString("translate");
                    encoder.WriteString("popup");
                    encoder.WriteString("jukeboxPopup");
                    encoder.WriteByte(MessageType);
                }
                encoder.WriteString(Message);
                encoder.WriteVarInt(0);
            }
            encoder.WriteString(XUID);
            encoder.WriteString(PlatformId);
            encoder.WriteString(FilteredMessage);
        }
    }
}
