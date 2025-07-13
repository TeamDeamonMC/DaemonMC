using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;

namespace DaemonMC.Network.Bedrock
{
    public class MobEquipment : Packet
    {
        public override int Id => (int) Info.Bedrock.MobEquipment;

        public long EntityId { get; set; } = 0;
        public Item Item { get; set; } = new Air();
        public byte Slot { get; set; } = 0;
        public byte SelectedSlot { get; set; } = 0;
        public byte ContainerId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            EntityId = decoder.ReadVarLong();
            Item = decoder.ReadItem();
            Slot = decoder.ReadByte();
            SelectedSlot = decoder.ReadByte();
            ContainerId = decoder.ReadByte();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteItem(Item);
            encoder.WriteByte(Slot);
            encoder.WriteByte(SelectedSlot);
            encoder.WriteByte(ContainerId);
        }
    }
}
