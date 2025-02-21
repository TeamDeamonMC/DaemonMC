namespace DaemonMC.Network.Bedrock
{
    public class Example : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Example;

        public int variable = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            variable = decoder.ReadInt();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteInt(variable);
        }
    }
}
