namespace DaemonMC.Network.Bedrock
{
    public class ServerToClientHandshake
    {
        public Info.Bedrock id = Info.Bedrock.ServerToClientHandshake;

        public string JWT = "";

        public void Decode(byte[] buffer)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteString(JWT);
            encoder.handlePacket();
        }
    }
}
