using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;

namespace DaemonMC.Network.Bedrock
{
    public class MobArmorEquipment : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.MobArmorEquipment;

        public long EntityId { get; set; } = 0;
        public Item Head { get; set; } = new Air();
        public Item Chest { get; set; } = new Air();
        public Item Legs { get; set; } = new Air();
        public Item Feet { get; set; } = new Air();
        public Item Body { get; set; } = new Air();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteItem(Head);
            encoder.WriteItem(Chest);
            encoder.WriteItem(Legs);
            encoder.WriteItem(Feet);
            encoder.WriteItem(Body);
        }
    }
}
