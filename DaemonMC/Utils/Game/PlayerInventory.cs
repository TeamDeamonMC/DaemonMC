using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;
using DaemonMC.Network.Bedrock;

namespace DaemonMC.Utils.Game
{
    public class PlayerInventory
    {
        internal readonly Player _player;
        public Dictionary<byte, Item> Inventory { get; protected set; } = new Dictionary<byte, Item>(); //0 - 9 hotbar, 10 - 35 inventory
        public Item Head { get; protected set; } = new Air();
        public Item Chest { get; protected set; } = new Air();
        public Item Legs { get; protected set; } = new Air();
        public Item Feets { get; protected set; } = new Air();
        public byte HandSlot { get; set; } = 0;

        public PlayerInventory(Player player)
        {
            _player = player;

            for (byte i = 0; i < 36; i++)
            {
                Inventory[i] = new Air();
            }
        }

        public void OnHead(Item item)
        {
            Set(ContainerId.Armor, 0, item);
        }

        public void OnChest(Item item)
        {
            Set(ContainerId.Armor, 1, item);
        }

        public void OnLegs(Item item)
        {
            Set(ContainerId.Armor, 2, item);
        }

        public void OnFeet(Item item)
        {
            Set(ContainerId.Armor, 3, item);
        }

        public Item GetHand()
        {
            if (Inventory.TryGetValue(HandSlot, out Item item))
            {
                return item;
            }
            else
            {
                return new Air();
            }
        }

        public bool Add(Item item)
        {
            for (byte i = 0; i < 35; i++)
            {
                var slot = Inventory[i];
                if (slot is not Air && slot.Name == item.Name && slot.Count < 64)
                {
                    int space = 64 - slot.Count;
                    int toAdd = Math.Min(space, item.Count);

                    slot.Count += (ushort)toAdd;
                    item.Count -= (ushort)toAdd;

                    Set(ContainerId.Inventory, (byte)i, slot);

                    if (item.Count == 0)
                    {
                        return true;
                    }
                }
            }

            for (byte i = 0; i < 35; i++)
            {
                if (Inventory[i] is Air)
                {
                    int toAdd = Math.Min(64, (int)item.Count);
                    var newItem = item.Clone();
                    newItem.Count = (ushort)toAdd;

                    Set(ContainerId.Inventory, (byte)i, newItem);

                    item.Count -= (ushort)toAdd;

                    if (item.Count == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Remove(byte slot)
        {
            Set(ContainerId.Inventory, slot, new Air());
        }

        public void Clear(Item specificItem = null)
        {
            for (byte i = 0; i < 35; i++)
            {
                if (Inventory[i] is Air)
                {
                    continue;
                }
                if (specificItem == null)
                {
                    Set(ContainerId.Inventory, i, new Air());
                }
                else
                {
                    if (Inventory[i].Name == specificItem.Name)
                    {
                        Set(ContainerId.Inventory, i, new Air());
                    }
                }
            }
        }

        public Item Get(byte slot)
        {
            if (Inventory.TryGetValue(slot, out Item item))
            {
                return item;
            }
            else
            {
                return new Air();
            }
        }

        public byte? LookFor(Item item)
        {
            return Inventory.Where(kvp => string.Equals(kvp.Value.Name, item.Name, StringComparison.OrdinalIgnoreCase)).Select(kvp => (byte?)kvp.Key).FirstOrDefault();
        }

        public void Set(ContainerId containerId, byte slot, Item item)
        {
            if (containerId == ContainerId.Armor)
            {
                switch (slot)
                {
                    case 0:
                        Head = item;
                        break;
                    case 1:
                        Chest = item;
                        break;
                    case 2:
                        Legs = item;
                        break;
                    case 3:
                        Feets = item;
                        break;
                    default:
                        Text.Log.warn($"Armor slot out of range {slot}. Expected range 0 - 3");
                        return;
                }
                var pk1 = new MobArmorEquipment
                {
                    EntityId = _player.EntityID,
                    Head = Head,
                    Chest = Chest,
                    Legs = Legs,
                    Feet = Feets,
                };
                _player.CurrentWorld.Send(pk1, _player.EntityID);
            }
            else if (containerId == ContainerId.Inventory)
            {
                if (slot > 35)
                {
                    Text.Log.warn($"Player inventory slot out of range {slot}. Expected range 0 - 35");
                    return;
                }
                Inventory[slot] = item;
            }
            else
            {
                Text.Log.warn($"Unknown Container Id {containerId}");
                return;
            }
            Send(containerId, slot, item);
        }

        public void Send(ContainerId containerId, byte slot, Item item)
        {
            var pk = new InventorySlot
            {
                ContainerID = (int)containerId,
                Slot = slot,
                Item = item,
            };
            _player.Send(pk);
        }
    }

    public enum ContainerId
    {
        None = -1,
        Inventory = 0,
        First = 1,
        Last = 100,
        Offhand = 119,
        Armor = 120,
        SelectionSlots = 122,
        PlayerOnlyUi = 124,
        Registry = 125,
    }
}
