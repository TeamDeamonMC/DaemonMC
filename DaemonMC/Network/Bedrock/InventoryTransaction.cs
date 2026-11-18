using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class InventoryTransaction : Packet
    {
        public override int Id => (int) Info.Bedrock.InventoryTransaction;

        public int RawID { get; set; } = 0;
        public List<LegacySlot> LegacySlots { get; set; } = new List<LegacySlot>();
        public Transaction Transaction { get; set; } = new Transaction();

        protected override void Decode(PacketDecoder decoder)
        {
            bool v1_26_30 = decoder.protocolVersion >= Info.v1_26_30;
            bool presenceFlags = v1_26_30 && decoder.protocolVersion < Info.v1_26_50;

            RawID = decoder.ReadSignedVarInt();
            LegacySlots = decoder.ReadLegacySlots(RawID, v1_26_30);

            if (presenceFlags && !decoder.ReadBool())
            {
                return;
            }

            Transaction = new Transaction();
            Transaction.LegacyRequestId = RawID;
            Transaction.LegacySlots = LegacySlots;

            Transaction.Type = (TransactionType)decoder.ReadVarInt();

            if (presenceFlags && !decoder.ReadBool())
            {
                return;
            }

            Transaction.Actions = decoder.ReadTransactionActions(v1_26_30);

            switch (Transaction.Type)
            {
                case TransactionType.NormalTransaction:
                case TransactionType.InventoryMismatch:
                    break;
                case TransactionType.ItemUseTransaction:
                    decoder.ReadItemUseData(Transaction, v1_26_30);
                    break;
                case TransactionType.ItemUseOnEntityTransaction:
                    Transaction.EntityId = decoder.ReadVarLong();
                    Transaction.ActionType = v1_26_30 ? decoder.ReadSignedVarInt() : decoder.ReadVarInt();
                    Transaction.Slot = decoder.ReadSignedVarInt();
                    if (decoder.protocolVersion >= Info.v1_26_60)
                    {
                        Transaction.Hand = decoder.ReadByte();
                    }
                    Transaction.Item = decoder.ReadItem(v1_26_30);
                    Transaction.PlayerPosition = decoder.ReadVec3();
                    Transaction.ClickPosition = decoder.ReadVec3();
                    break;
                case TransactionType.ItemReleaseTransaction:
                    Transaction.ActionType = v1_26_30 ? decoder.ReadSignedVarInt() : decoder.ReadVarInt();
                    Transaction.Slot = decoder.ReadSignedVarInt();
                    Transaction.Item = decoder.ReadItem(v1_26_30);
                    Transaction.HeadPosition = decoder.ReadVec3();
                    if (decoder.protocolVersion >= Info.v1_26_60)
                    {
                        Transaction.Hand = decoder.ReadByte();
                    }
                    break;
                default:
                    Log.error($"Unknown transaction type {Transaction.Type}");
                    break;
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
