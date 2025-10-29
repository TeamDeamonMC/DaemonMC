using System.Reflection;

namespace DaemonMC.Blocks
{
    public class BlockPalette
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

                var stateOptions = new Dictionary<string, object[]>();
                foreach (var state in blockInstance.States)
                {
                    if (BlockStates.States.TryGetValue(state.Key, out var possibleValues))
                    {
                        stateOptions[state.Key] = possibleValues;
                    }
                    else
                    {
                        stateOptions[state.Key] = new object[] { state.Value };
                    }
                }

                foreach (var combination in GetCombinations(stateOptions))
                {
                    Block newBlock = (Block)Activator.CreateInstance(type);

                    foreach (var kvp in combination)
                    {
                        newBlock.States[kvp.Key] = kvp.Value;
                    }

                    int hash = newBlock.GetHash();
                    blockHashes[hash] = newBlock;
                }
            }
        }

        public static Block? GetBlock(string blockName)
        {
            var block = blockHashes.Values.FirstOrDefault(i => string.Equals(i.Name, blockName, StringComparison.OrdinalIgnoreCase));
            return block == null ? null : block.Clone();
        }

        private static IEnumerable<Dictionary<string, object>> GetCombinations(Dictionary<string, object[]> options)
        {
            var keys = options.Keys.ToList();

            IEnumerable<Dictionary<string, object>> Recurse(int index)
            {
                if (index == keys.Count)
                {
                    yield return new Dictionary<string, object>();
                }
                else
                {
                    string key = keys[index];
                    foreach (var value in options[key])
                    {
                        foreach (var rest in Recurse(index + 1))
                        {
                            rest[key] = value;
                            yield return new Dictionary<string, object>(rest);
                        }
                    }
                }
            }

            return Recurse(0);
        }
    }
}
