using System.Numerics;
using DaemonMC.Items;

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
        public Item Item { get; set; }
        public int Face { get; set; }
        public int Slot { get; set; }
    }

    public class TransactionAction
    {
        public SourceType Source { get; set; }
        public int ContainerID { get; set; }
        public int Slot { get; set; }
        public Item ItemFrom { get; set; }
        public Item ItemTo { get; set; }
    }

    public class FullContainerName
    {
        public byte ContainerName { get; set; } = 0;
        public int? DynamicId { get; set; } = 0;
    }

    public class Actions
    {
        public byte ActionsType { get; set; }
        public byte Amount { get; set; }
        public ItemStackRequestSlotInfo Source { get; set; }
        public ItemStackRequestSlotInfo Destination { get; set; }
    }

    public class ItemStackRequestSlotInfo
    {
        public FullContainerName ContainerName { get; set; }
        public byte Slot { get; set; }
        public int NetIdVariant { get; set; }
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
