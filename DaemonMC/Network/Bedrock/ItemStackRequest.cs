using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class ItemStackRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.ItemStackRequest;

        public List<ItemStack> ItemStack { get; set; } = new List<ItemStack>();

        protected override void Decode(PacketDecoder decoder)
        {
            ItemStack = decoder.ReadItemStack();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
