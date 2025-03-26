using DaemonMC.Items;
using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class ItemRegistry : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ItemRegistry;

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
                encoder.WriteBool(false); //components
                encoder.WriteVarInt(2);
                encoder.WriteCompoundTag(new NbtCompound(""));
            }
        }
    }
}
