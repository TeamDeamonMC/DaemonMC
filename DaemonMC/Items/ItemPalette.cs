using System.Reflection;
using DaemonMC.Items;

namespace DaemonMC.Blocks
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
    }
}
