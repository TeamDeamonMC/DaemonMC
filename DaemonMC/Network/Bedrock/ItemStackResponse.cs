using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class ItemStackResponse : Packet
    {
        public override int Id => (int) Info.Bedrock.ItemStackResponse;

        public List<ItemStackResponseInfo> ItemStack { get; set; } = new List<ItemStackResponseInfo>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteItemStackResponse(ItemStack);
        }
    }
}
