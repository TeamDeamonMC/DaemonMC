using DaemonMC.Items;
using DaemonMC.Items.VanillaItems;
using DaemonMC.Network.Bedrock;

namespace DaemonMC.Utils.Game
{
    public class PlayerInventory
    {
        internal readonly Player _player;
        public Dictionary<byte, Item> Inventory { get; protected set; } = new Dictionary<byte, Item>(); //0 - 8 hotbar, 8 - 35 inventory
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
            Set(120, 0, item);
        }

        public void OnChest(Item item)
        {
            Set(120, 1, item);
        }

        public void OnLegs(Item item)
        {
            Set(120, 2, item);
        }

        public void OnFeets(Item item)
        {
            Set(120, 3, item);
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

        public void Set(byte containerId, byte slot, Item item)
        {
            if (containerId == 120)
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
            else if (containerId == 0)
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

        public void Send(byte containerId, byte slot, Item item)
        {
            var pk = new InventorySlot
            {
                ContainerID = containerId,
                Slot = slot,
                Item = item,
            };
            _player.Send(pk);
        }
    }
}
