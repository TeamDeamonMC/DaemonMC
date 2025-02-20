using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class UpdateAttributes : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.UpdateAttributes;

        public long EntityId = 0;
        public List<AttributeValue> Attributes = new List<AttributeValue>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong((ulong) EntityId);
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
        }
    }
}
