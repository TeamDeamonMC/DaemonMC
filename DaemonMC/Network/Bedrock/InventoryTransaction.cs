using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class InventoryTransaction : Packet
    {
        public override int Id => (int) Info.Bedrock.InventoryTransaction;

        public int RawID { get; set; } = 0;
        public bool hasLegacySlots { get; set; } = true;
        public List<LegacySlot> LegacySlots { get; set; } = new List<LegacySlot>();
        public Transaction Transaction { get; set; } = new Transaction();

        protected override void Decode(PacketDecoder decoder)
        {
            RawID = decoder.ReadVarInt();

            if (decoder.protocolVersion >= Info.v1_26_30)
            {
                bool hasLegacySlots = decoder.ReadBool();
            }

            if (hasLegacySlots && RawID != 0)
            {
                int legacyCount = decoder.ReadVarInt();

                for (int i = 0; i < legacyCount; i++)
                {
                    LegacySlot legacySlot = new LegacySlot();
                    legacySlot.ContainerId = decoder.ReadByte();
                    for (int a = 0; a < decoder.ReadVarInt(); a++)
                    {
                        legacySlot.Slot[i] = decoder.ReadByte();
                    }
                    LegacySlots.Add(legacySlot);
                }
            }

            if (decoder.protocolVersion >= Info.v1_26_30)
            {
                if (!decoder.ReadBool())
                {
                    return;
                }
            }
            
            Transaction = new Transaction();

            Transaction.Type = (TransactionType)decoder.ReadVarInt();

            if (decoder.protocolVersion >= Info.v1_26_30)
            {
                bool hasActions = decoder.ReadBool();
                if (!hasActions)
                    return;
            }
            
            int count = decoder.ReadVarInt();

            for (int b = 0; b < count; b++)
            {
                TransactionAction action = new TransactionAction();

                action.Source = (SourceType)decoder.ReadVarInt();

                if (action.Source == SourceType.ContainerInventory || action.Source == SourceType.WorldInteraction)
                {
                    action.ContainerID = decoder.ReadVarInt();
                }

                action.Slot = decoder.ReadVarInt();
                action.ItemFrom = decoder.ReadItem();
                action.ItemTo = decoder.ReadItem();
                Transaction.Actions.Add(action);
            }

            switch (Transaction.Type)
            {
                case TransactionType.NormalTransaction:
                case TransactionType.InventoryMismatch:
                    break;
                case TransactionType.ItemUseTransaction:
                    Transaction.ActionType = decoder.ReadSignedVarInt();
                    Transaction.TriggerType = decoder.protocolVersion >= Info.v1_26_30 ? decoder.ReadByte(): decoder.ReadSignedVarInt();
                    Transaction.BlockPosition = decoder.ReadBlockNetPos();
                    Transaction.Face = decoder.protocolVersion >= Info.v1_26_30 ? decoder.ReadByte() : decoder.ReadSignedVarInt();
                    Transaction.Slot = decoder.ReadSignedVarInt();
                    Transaction.Item = decoder.ReadItem(decoder.protocolVersion >= Info.v1_26_30 ? true : false);
                    Transaction.PlayerPosition = decoder.ReadVec3();
                    Transaction.ClickPosition = decoder.ReadVec3();
                    Transaction.BlockRuntimeId = decoder.ReadVarInt();
                    Transaction.ClientPrediction = decoder.protocolVersion >= Info.v1_26_30 ? decoder.ReadByte() : decoder.ReadVarInt();
                    if (decoder.protocolVersion >= Info.v1_26_10)
                    {
                        Transaction.ClientPrediction = decoder.ReadByte();
                    }
                    break;
                case TransactionType.ItemUseOnEntityTransaction:
                    Transaction.EntityId = decoder.ReadVarLong();
                    Transaction.ActionType = decoder.ReadSignedVarInt();
                    Transaction.Slot = decoder.ReadSignedVarInt();
                    Transaction.Item = decoder.ReadItem(decoder.protocolVersion >= Info.v1_26_30 ? true : false);
                    Transaction.PlayerPosition = decoder.ReadVec3();
                    Transaction.ClickPosition = decoder.ReadVec3();
                    break;
                case TransactionType.ItemReleaseTransaction:
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
