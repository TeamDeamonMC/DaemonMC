using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class TextMessagePacket
    {
        public byte messageType { get; set; }
        public bool Localized { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
    }

    public class TextMessage
    {
        public const int id = 9;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new TextMessagePacket
            {
                messageType = decoder.ReadByte(),
                Localized = decoder.ReadBool(),
                Username = decoder.ReadString(),
                Message = decoder.ReadString()
            };
            BedrockPacketProcessor.Text(packet, decoder.endpoint);
        }

        public static void Encode(TextMessagePacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteByte(1);
            encoder.WriteBool(fields.Localized);
            encoder.WriteString(fields.Username);
            encoder.WriteString(fields.Message);

            encoder.WriteString(""); //xuid
            encoder.WriteString(""); //platform id
            encoder.WriteString(""); //filtered msg
            encoder.handlePacket();
        }
    }
}
