namespace DaemonMC.Network.Bedrock
{
    public class CreativeContent : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.CreativeContent;
        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (encoder.protocolVersion >= Info.v1_21_60)
            {
                encoder.WriteVarInt(0); //groups
            }
            encoder.WriteVarInt(0);
        }
    }
}
