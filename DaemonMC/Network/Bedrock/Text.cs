namespace DaemonMC.Network.Bedrock
{
    public class TextMessage
    {
        public Info.Bedrock id = Info.Bedrock.TextMessage;

        public byte messageType = 0;
        public bool Localized = false;
        public string Username = "";
        public string Message = "";

        public void Decode(PacketDecoder decoder)
        {
            var packet = new TextMessage
            {
                messageType = decoder.ReadByte(),
                Localized = decoder.ReadBool(),
                Username = decoder.ReadString(),
                Message = decoder.ReadString()
            };
            BedrockPacketProcessor.Text(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteByte(1);
            encoder.WriteBool(Localized);
            encoder.WriteString(Username);
            encoder.WriteString(Message);

            encoder.WriteString(""); //xuid
            encoder.WriteString(""); //platform id
            encoder.WriteString(""); //filtered msg
            encoder.handlePacket();
        }
    }
}
