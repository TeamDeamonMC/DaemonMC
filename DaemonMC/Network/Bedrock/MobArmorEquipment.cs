using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;

namespace DaemonMC.Network.Bedrock
{
    public class MobArmorEquipment : Packet
    {
        public override int Id => (int) Info.Bedrock.MobArmorEquipment;

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
            if (encoder.protocolVersion >= Info.v1_26_30)
            {
                encoder.WriteNetItemStack(Head);
                encoder.WriteNetItemStack(Chest);
                encoder.WriteNetItemStack(Legs);
                encoder.WriteNetItemStack(Feet);
                encoder.WriteNetItemStack(Body);
            }
            else
            {
                encoder.WriteItemStack(Head);
                encoder.WriteItemStack(Chest);
                encoder.WriteItemStack(Legs);
                encoder.WriteItemStack(Feet);
                encoder.WriteItemStack(Body);
            }
        }
    }
}
