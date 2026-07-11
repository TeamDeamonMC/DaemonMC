using DaemonMC.Items;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;
using DaemonMC.Plugin;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Handler
{
    public class InventoryTransactionHandler
    {
        public static void TakeAction(Player player, TakeAction action, ItemStack itemStack)
        {
            if (action.Destination.ContainerName.ContainerName == ContainerEnumName.CursorContainer)
            {
                Item sourceItem = new Items.VanillaItems.Air();

                if (action.Source.ContainerName.ContainerName == ContainerEnumName.HotbarContainer || action.Source.ContainerName.ContainerName == ContainerEnumName.InventoryContainer) //player inventory
                {
                    sourceItem = player.Inventory.Get(action.Source.Slot);
                }
                else if (action.Source.ContainerName.ContainerName == ContainerEnumName.CreatedOutputContainer)
                {
                    sourceItem = itemStack.OutputContainer;
                }
                else
                {
                    Log.error("Inventory error 6");
                    return;
                }

                var cursorItem = sourceItem.Clone();
                var count = action.Amount;

                Log.debug($"[TakeAction] Requested {count} items from slot {action.Source.Slot} that have {sourceItem.Count} {sourceItem.Name}");

                if (!PluginManager.InventoryAction(player, action, sourceItem, new Items.VanillaItems.Air()))
                {
                    Log.debug($"[TakeAction] Cancelled by plugin"); //todo figure out how to update inventory correctly while UI is open
                    player.Inventory.Send(ContainerId.PlayerOnlyUi, 0, new Items.VanillaItems.Air(), ContainerEnumName.CursorContainer);
                    player.Inventory.Send(ContainerId.Inventory, action.Source.Slot, sourceItem);
                    return;
                }

                cursorItem.Count = count;
                player.Inventory.Cursor = cursorItem;

                if (action.Source.ContainerName.ContainerName == ContainerEnumName.CreatedOutputContainer)
                {
                    return;
                }

                sourceItem.Count -= count;

                Item destinationItem = new Items.VanillaItems.Air();

                if (sourceItem.Count > 0)
                {
                    destinationItem = sourceItem;
                }

                player.Inventory.Set(ContainerId.Inventory, action.Source.Slot, destinationItem);
                Log.debug($"[TakeAction] Cursor have now {cursorItem.Count} {cursorItem.Name}. Source slot {action.Source.Slot} have now {destinationItem.Count} {destinationItem.Name}");
            }
            else
            {
                Log.error("Inventory error 2");
            }
        }

        public static void PlaceAction(Player player, PlaceAction action, ItemStack itemStack)
        {
            if (action.Destination.ContainerName.ContainerName == ContainerEnumName.InventoryContainer || action.Destination.ContainerName.ContainerName == ContainerEnumName.HotbarContainer || action.Destination.ContainerName.ContainerName == ContainerEnumName.CombinedHotbarAndInventoryContainer)
            {
                Item sourceItem = new Items.VanillaItems.Air();

                if (action.Source.ContainerName.ContainerName == ContainerEnumName.CursorContainer)
                {
                    sourceItem = player.Inventory.Cursor;
                }
                else if (action.Source.ContainerName.ContainerName == ContainerEnumName.CreatedOutputContainer)
                {
                    sourceItem = itemStack.OutputContainer;
                }
                else
                {
                    Log.error("Inventory error 5");
                    return;
                }

                Item destinationItem = player.Inventory.Get(action.Destination.Slot);
                var count = action.Amount;

                Log.debug($"[PlaceAction] Requested {count} items from inventory {action.Source.ContainerName.ContainerName} {action.Source.Slot} slot that have {sourceItem.Count} {sourceItem.Name}");

                if (!PluginManager.InventoryAction(player, action, new Items.VanillaItems.Air(), destinationItem))
                {
                    Log.debug($"[PlaceAction] Cancelled by plugin"); //todo even more bugged
                    player.Inventory.Send(ContainerId.PlayerOnlyUi, 0, sourceItem, ContainerEnumName.CursorContainer);
                    player.Inventory.Send(ContainerId.Inventory, action.Destination.Slot, destinationItem);
                    return;
                }

                if (sourceItem is Items.VanillaItems.Air)
                {
                    Log.debug($"[PlaceAction] Cancelled because cursor inventory is empty");
                    return;
                }

                if (action.Source.ContainerName.ContainerName == ContainerEnumName.CursorContainer) sourceItem.Count -= count;

                if (destinationItem is Items.VanillaItems.Air) //empty slot
                {
                    destinationItem = sourceItem.Clone();
                    destinationItem.Count = count;
                    Log.debug($"[PlaceAction] Adding new {destinationItem.Name} with count {destinationItem.Count} in empty slot {action.Destination.Slot}");
                }
                else
                {
                    Log.debug($"[PlaceAction] {destinationItem.Name} with count {destinationItem.Count} already in slot {action.Destination.Slot}. Adding more {count}. Expected total {destinationItem.Count + count}");
                    destinationItem.Count += count;
                }

                if (action.Source.ContainerName.ContainerName == ContainerEnumName.CursorContainer)
                {
                    if (sourceItem.Count <= 0)
                    {
                        sourceItem = new Items.VanillaItems.Air();
                    }

                    player.Inventory.Cursor = sourceItem;
                }

                player.Inventory.Set(ContainerId.Inventory, action.Destination.Slot, destinationItem);
                Log.debug($"[PlaceAction] {action.Source.ContainerName.ContainerName} {action.Source.Slot} slot have now {sourceItem.Count} {sourceItem.Name}. Destination slot {action.Destination.Slot} have now {destinationItem.Count} {destinationItem.Name}");
            }
            else
            {
                Log.error("Inventory error 3");
            }
        }

        public static void DestoryAction(Player player, DestoryAction action)
        {
            if (action.Source.ContainerName.ContainerName == ContainerEnumName.CursorContainer)
            {
                Item sourceItem = player.Inventory.Cursor;
                var count = action.Amount;

                sourceItem.Count -= count;

                if (sourceItem.Count <= 0)
                {
                    sourceItem = new Items.VanillaItems.Air();
                }

                player.Inventory.Cursor = sourceItem;
                Log.debug($"[DestroyAction] Cursor have now {sourceItem.Count} {sourceItem.Name}.");
            }
            else
            {
                Log.error("Inventory error 4");
            }
        }
        public static void CreaftCreativeAction(Player player, CraftCreativeAction action, ItemStack itemStack)
        {
            var item = CreativeContentManager.GetItemByNetId(action.ItemId);

            if (item != null)
            {
                itemStack.OutputContainer = item;
            }
        }

        public static void DeclineRequest(Player player, int requestId)
        {
            var packet = new ItemStackResponse();
            packet.ItemStack.Add(new ItemStackResponseInfo() { RequestId = requestId, Result = 1 });
            player.Send(packet);
        }
    }
}
