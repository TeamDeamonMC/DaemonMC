using System.Reflection;
using DaemonMC.Items.VanillaItems;

namespace DaemonMC.Items
{
    public class ItemPalette
    {
        public static Dictionary<short, Item> items { get; protected set; } = new Dictionary<short, Item>();

        public static void buildPalette()
        {
            var itemTypes = Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Item)));

            foreach (var type in itemTypes)
            {
                Item itemInstance = (Item)Activator.CreateInstance(type);

                items[itemInstance.Id] = itemInstance;
            }
        }

        public static Item? GetItem(string itemName)
        {
            var item = items.Values.FirstOrDefault(i => string.Equals(i.Name, itemName, StringComparison.OrdinalIgnoreCase));
            return item == null ? null : item.Clone();
        }
    }
}
