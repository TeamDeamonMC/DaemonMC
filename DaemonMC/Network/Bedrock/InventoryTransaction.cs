using System.Security.AccessControl;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class InventoryTransaction : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.InventoryTransaction;

        public int RawID { get; set; } = 0;
        public int TransactionType { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            RawID = decoder.ReadSignedVarInt();
            TransactionType = decoder.ReadVarInt();
            if (TransactionType == 3)
            {

            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
