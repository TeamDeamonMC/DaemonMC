namespace DaemonMC.Network.Bedrock
{
    public class TextMessage : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.TextMessage;

        public byte messageType = 0;
        public bool Localized = false;
        public string Username = "";
        public string Message = "";

        protected override void Decode(PacketDecoder decoder)
        {
            messageType = decoder.ReadByte();
            Localized = decoder.ReadBool();
            Username = decoder.ReadString();
            Message = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(messageType);
            encoder.WriteBool(Localized);
            encoder.WriteString(Username);
            encoder.WriteString(Message);

            encoder.WriteString(""); //xuid
            encoder.WriteString(""); //platform id
            encoder.WriteString(""); //filtered msg
        }
    }
}
