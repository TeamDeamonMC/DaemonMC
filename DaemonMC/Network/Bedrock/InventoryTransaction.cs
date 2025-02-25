using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class InventoryTransaction : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.InventoryTransaction;

        public int RawID { get; set; } = 0;
        public Transaction Transaction { get; set; } = new Transaction();

        protected override void Decode(PacketDecoder decoder)
        {
            RawID = decoder.ReadSignedVarInt();

            Transaction = new Transaction();

            Transaction.Type = (TransactionType)decoder.ReadVarInt();

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
                decoder.ReadSignedVarInt();
                decoder.ReadSignedVarInt();
                Transaction.Actions.Add(action);
            }

            if (Transaction.Type == TransactionType.ItemUseOnEntityTransaction)
            {
                Transaction.EntityId = decoder.ReadVarLong();
                Transaction.ActionType = decoder.ReadVarInt();
                Transaction.Slot = decoder.ReadSignedVarInt();
                decoder.ReadSignedVarInt(); //todo items
                Transaction.FromPosition = decoder.ReadVec3();
                Transaction.Position = decoder.ReadVec3();
            }

        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
