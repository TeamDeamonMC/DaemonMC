namespace DaemonMC.Network.Bedrock
{
    public class Example
    {
        public Info.Bedrock id = Info.Bedrock.Example;

        public int variable = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.handlePacket();
        }
    }
}
