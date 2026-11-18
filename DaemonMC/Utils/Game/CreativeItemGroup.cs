using DaemonMC.Items;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Text;

namespace DaemonMC.Utils.Game;

public class CreativeItemGroup
{
    public CreativeCategoryType Category { get; set; }
    public string Name { get; set; } = string.Empty;
    public Item? Icon { get; set; }
    public List<Item> Items { get; set; } = new List<Item>();

    public CreativeItemGroup() { }

    public CreativeItemGroup(CreativeCategoryType category)
    {
        Category = category;
    }

    public CreativeItemGroup(CreativeCategoryType category, string name, Item icon)
    {
        Category = category;
        Name = name;
        Icon = icon;
    }

    public void AddItem(Item item)
    {
        Items.Add(item);
    }

    public Item GetItemByName(string name)
    {
        var item = Items.FirstOrDefault(g => g.Name == name);

        if (item == null)
        {
            return new Items.VanillaItems.Air();
        }

        return item;
    }

    public void RemoveItem(Item item)
    {
        if (!Items.Remove(item))
        {
            Log.warn($"[CreativeContentManager] Couldn't remove item with name {item.Name}.Item doesn't exist.");
        }
    }

    public void RemoveItem(string itemName)
    {
        Item? item = Items.FirstOrDefault(g => g.Name == itemName);
        if (item == null)
        {
            Log.warn($"[CreativeContentManager] Couldn't remove item with name {itemName}.Item doesn't exist.");
        }
        else
        {
            Items.Remove(item);
        }
    }
}