using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class UpdateAttributes : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.UpdateAttributes;

        public long EntityId { get; set; } = 0;
        public List<AttributeValue> Attributes { get; set; } = new List<AttributeValue>();
        public long Tick { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WritePlayerAttributes(Attributes);
            encoder.WriteVarLong(Tick);
        }
    }
}
