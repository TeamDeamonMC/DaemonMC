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
            if (encoder.protocolVersion >= Info.v1_21_60)
            {
                encoder.WriteVarInt(0); //groups
            }
            encoder.WriteVarInt(0);
            encoder.handlePacket();
        }
    }
}
