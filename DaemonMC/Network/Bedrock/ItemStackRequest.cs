using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class ItemStackRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.ItemStackRequest;

        public List<ItemStack> ItemStack { get; set; } = new List<ItemStack>();

        protected override void Decode(PacketDecoder decoder)
        {
            int size = decoder.ReadVarInt();
            for (int i = 0; i < size; i++)
            {
                try
                {
                    ItemStack.Add(decoder.ReadItemStackRequest());
                }
                catch (NotSupportedException e)
                {
                    Log.warn($"ItemStackRequest: {e.Message}");
                    break;
                }
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
