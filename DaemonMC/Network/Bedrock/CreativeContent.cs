namespace DaemonMC.Network.Bedrock
{
    public class CreativeContent
    {
        public Info.Bedrock id = Info.Bedrock.CreativeContent;
        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt((int) id);
            encoder.WriteVarInt(0);
            encoder.handlePacket();
        }
    }
}
