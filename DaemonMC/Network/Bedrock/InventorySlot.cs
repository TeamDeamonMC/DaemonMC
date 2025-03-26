using DaemonMC.Items;

namespace DaemonMC.Network.Bedrock
{
    public class InventorySlot : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.InventorySlot;

        public int ContainerID { get; set; } = 0;
        public int Slot { get; set; } = 0;
        public Item Item { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(ContainerID);
            encoder.WriteVarInt(Slot);

            encoder.WriteByte(0);
            encoder.WriteBool(false);

            encoder.WriteSignedVarInt(0);



            encoder.WriteSignedVarInt(Item.Id);
            encoder.WriteShort(1);
            encoder.WriteVarInt(0);

            encoder.WriteBool(false);

            encoder.WriteSignedVarInt(0);
            encoder.WriteVarInt(0);
        }
    }
}
