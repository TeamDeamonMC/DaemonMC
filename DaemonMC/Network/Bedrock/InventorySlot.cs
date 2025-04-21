using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class InventorySlot : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.InventorySlot;

        public int ContainerID { get; set; } = 0;
        public int Slot { get; set; } = 0;
        public FullContainerName ContainerName { get; set; } = new FullContainerName();
        public Item StorageItem { get; set; } = new Air();
        public Item Item { get; set; } = new Air();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(ContainerID);
            encoder.WriteVarInt(Slot);
            encoder.WriteContainerName(ContainerName);
            encoder.WriteItem(StorageItem);
            encoder.WriteItem(Item);
        }
    }
}
