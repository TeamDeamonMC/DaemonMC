namespace DaemonMC.Network.Bedrock
{
    public class UpdateAttributes
    {
        public Info.Bedrock id = Info.Bedrock.UpdateAttributes;

        public ulong EntityId = 0;
        public List<Attribute> Attributes = new List<Attribute>();

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong(EntityId);
            encoder.WriteVarInt(Attributes.Count);
            foreach (var attribute in Attributes)
            {
                encoder.WriteFloat(attribute.MinValue);
                encoder.WriteFloat(attribute.MaxValue);
                encoder.WriteFloat(attribute.CurrentValue);
                encoder.WriteFloat(attribute.DefaultMinValue);
                encoder.WriteFloat(attribute.DefaultMaxValue);
                encoder.WriteFloat(attribute.DefaultValue);
                encoder.WriteString(attribute.Name);
                encoder.WriteVarInt(0); //todo modifiers
            }
            encoder.WriteVarLong(0); //todo tick
            encoder.handlePacket();
        }
    }
}
