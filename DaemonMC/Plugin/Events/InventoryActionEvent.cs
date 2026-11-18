using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;
using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;

namespace DaemonMC.Plugin.Events;

public class InventoryActionEvent(Player player, Actions actions, Item sourceItem, Item destinationItem) : Event {

    private Player Player { get; } = player;
    private Actions actions { get; } = actions;
    private Item SourceItem { get; } = sourceItem;
    private Item DestinationItem { get; } = destinationItem;

    public Player GetPlayer() {
        return Player;
    }

    public Actions GetAction()
    {
        return actions;
    }

    public ItemStackRequestActionType GetActionType()
    {
        return actions.ActionsType;
    }

    public byte GetSourceSlot()
    {
        return actions.Source.Slot;
    }

    public byte GetDestinationSlot()
    {
        return actions.Destination.Slot;
    }

    public Item GetSourceItem()
    {
        return SourceItem;
    }

    public Item GetDestinationItem()
    {
        return DestinationItem;
    }

    public ContainerEnumName GetSourceInventory()
    {
        return actions.Source.ContainerName.ContainerName;
    }

    public ContainerEnumName GetDestinationInventory()
    {
        return actions.Destination.ContainerName.ContainerName;
    }
}
