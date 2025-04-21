using System.Numerics;

namespace DaemonMC.Utils.Game
{
    public class Transaction
    {
        public TransactionType Type { get; set; }
        public List<TransactionAction> Actions { get; set; } = new List<TransactionAction>();
        public int ActionType { get; set; }
        public int TriggerType { get; set; }
        public long EntityId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 FromPosition { get; set; }
        public int Face { get; set; }
        public int Slot { get; set; }
    }

    public class TransactionAction
    {
        public SourceType Source { get; set; }
        public int ContainerID { get; set; }
        public int Slot { get; set; }
    }

    public class FullContainerName
    {
        public byte ContainerName { get; set; } = 0;
        public int DynamicId { get; set; } = 0;
    }

    public enum TransactionType
    {
        NormalTransaction,
        InventoryMismatch,
        ItemUseTransaction,
        ItemUseOnEntityTransaction,
        ItemReleaseTransaction
    }

    public enum SourceType
    {
        ContainerInventory,
        GlobalInventory,
        WorldInteraction,
        CreativeInventory
    }
}
