namespace DaemonMC.Network.Bedrock
{
    public class Disconnect
    {
        public Info.Bedrock id = Info.Bedrock.Disconnect;

        public string message = "";

        public void Decode(byte[] buffer)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarInt(0);
            encoder.WriteBool(false);
            encoder.WriteString(message);
            encoder.WriteString("");
            encoder.handlePacket();
        }
    }
}
