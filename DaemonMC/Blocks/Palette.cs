using System.Reflection;

namespace DaemonMC.Blocks
{
    public class Palette
    {
        public static Dictionary<int, Block> blockHashes { get; protected set; } = new Dictionary<int, Block>();

        public static void buildPalette()
        {
            var blockTypes = Assembly.GetExecutingAssembly()
                                     .GetTypes()
                                     .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Block)));

            foreach (var type in blockTypes)
            {
                Block blockInstance = (Block)Activator.CreateInstance(type);

                int hash = blockInstance.GetHash();

                blockHashes[hash] = blockInstance;
            }
        }
    }
}
