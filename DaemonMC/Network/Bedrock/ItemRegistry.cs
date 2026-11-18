using DaemonMC.Items;
using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class ItemRegistry : Packet
    {
        public override int Id => (int) Info.Bedrock.ItemRegistry;

        public Dictionary<short, Item> Items { get; set; } = new Dictionary<short, Item>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(Items.Count());
            foreach (var item in Items)
            {
                encoder.WriteString(item.Value.Name);
                encoder.WriteShort((ushort)item.Value.Id);
                encoder.WriteBool(item.Value.ComponentBased);
                encoder.WriteSignedVarInt(item.Value.Version);
                encoder.WriteCompoundTag(item.Value.ComponentData != "" ? new NbtCompound(ToDataTypes.Base64ToNbt(item.Value.ComponentData, true, false)) : new NbtCompound(""));
            }
        }
    }
}
