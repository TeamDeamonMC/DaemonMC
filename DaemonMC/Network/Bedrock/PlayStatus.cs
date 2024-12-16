namespace DaemonMC.Network.Bedrock
{
    public class PlayStatus
    {
        public Info.Bedrock id = Info.Bedrock.PlayStatus;

        public int status = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteIntBE(status);
            encoder.handlePacket();
        }
    }
}
