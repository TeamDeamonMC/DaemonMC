namespace DaemonMC.Network.Bedrock
{
    public class Example : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Example;

        public int Variable { get; set; } = 0;
        public string AnotherVariable { get; set; } = "";
        public Guid SomeNewID { get; set; } = new Guid();

        protected override void Decode(PacketDecoder decoder)
        {
            Variable = decoder.ReadInt();
            AnotherVariable = decoder.ReadString();
            if (decoder.protocolVersion >= Info.v1_21_60) //packets have to be backward compatible 
            {
                decoder.ReadUUID();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteInt(Variable);
            encoder.WriteString(AnotherVariable);
            if (encoder.protocolVersion >= Info.v1_21_60)
            {
                encoder.WriteUUID(SomeNewID);
            }
        }
    }
}
