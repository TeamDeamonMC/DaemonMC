namespace DaemonMC.Network.Bedrock
{
    public class CreativeContent : Packet
    {
        public override int Id => (int) Info.Bedrock.CreativeContent;
        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(0); //groups
            encoder.WriteVarInt(0); //items
        }
    }
}
