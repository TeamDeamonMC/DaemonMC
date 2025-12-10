using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using fNbt;

namespace Test
{
    [TestClass]
    public class GenerateVanillaData
    {
        [TestMethod]
        public void Blocks()
        {
            HashSet<string> generatedBlocks = new HashSet<string>();
            using (var stream = File.OpenRead("canonical_block_states.nbt")) //https://github.com/pmmp/BedrockData/blob/master/canonical_block_states.nbt
            {
                while (stream.Position < stream.Length)
                {
                    var compound = ReadNBT(stream);

                    string blockName = compound.Get<NbtString>("name").StringValue;
                    string className = FixCase2(blockName.Replace("minecraft:", ""));

                    if (generatedBlocks.Contains(blockName))
                    {
                        continue;
                    }

                    generatedBlocks.Add(blockName);

                    var statesCompound = compound.Get<NbtCompound>("states");
                    Dictionary<string, string> states = new Dictionary<string, string>();

                    foreach (var tag in statesCompound.Tags)
                    {
                        if (tag is NbtInt intTag)
                            states[intTag.Name] = intTag.IntValue.ToString();
                        else if (tag is NbtByte byteTag)
                            states[byteTag.Name] = $"(byte){byteTag.ByteValue}";
                        else if (tag is NbtString stringTag)
                            states[stringTag.Name] = $"\"{stringTag.StringValue}\"";
                    }

                    string classContent = BlockClassBuilder(className, blockName, states);

                    Directory.CreateDirectory("VanillaBlocks");

                    string filePath = Path.Combine("VanillaBlocks", $"{className}.cs");
                    File.WriteAllText(filePath, classContent, Encoding.UTF8);

                    Console.WriteLine($"Generated: {filePath}");
                }
            }
        }

        [TestMethod]
        public void Items()
        {
            string json = File.ReadAllText("required_item_list.json"); //https://github.com/pmmp/BedrockData/master/required_item_list.json
            var doc = JsonDocument.Parse(json);

            Directory.CreateDirectory("VanillaItems");

            foreach (var item in doc.RootElement.EnumerateObject())
            {
                string name = item.Name;

                var obj = item.Value;

                int id = obj.GetProperty("runtime_id").GetInt32();
                int version = obj.GetProperty("version").GetInt32();
                bool componentBased = obj.GetProperty("component_based").GetBoolean();

                string className = FixCase(name.Split(':')[1]);

                string content = $@"
namespace DaemonMC.Items.VanillaItems
{{
    public class {className} : Item
    {{
        public {className}()
        {{
            Name = ""{name}"";
            Id = {id};
            Version = {version};
            ComponentBased = {(componentBased ? "true" : "false")};
        }}
    }}
}}";

                File.WriteAllText($"VanillaItems/{className}.cs", content.Trim());
                Console.WriteLine($"Generated: VanillaItems/{className}.cs");
            }

            Console.WriteLine("Done");
        }


        [TestMethod]
        public void Entities()
        {
            HashSet<string> generatedEntities = new HashSet<string>();

            using (var stream = File.OpenRead("entity_identifiers.nbt")) //https://github.com/pmmp/BedrockData/blob/master/entity_identifiers.nbt
            {
                while (stream.Position < stream.Length)
                {
                    var compound = ReadNBT(stream);

                    var idList = compound.Get<NbtList>("idlist");

                    foreach (var tag in idList)
                    {
                        if (tag is NbtCompound entityData)
                        {
                            string entityId = entityData.Get<NbtString>("id").StringValue;

                            if (string.IsNullOrWhiteSpace(entityId) || generatedEntities.Contains(entityId))
                            {
                                continue;
                            }

                            generatedEntities.Add(entityId);

                            string className = FixCase(entityId.Replace("minecraft:", ""));

                            string classContent = EntityClassBuilder(className, entityId);

                            Directory.CreateDirectory("VanillaEntities");

                            string filePath = Path.Combine("VanillaEntities", $"{className}.cs");
                            File.WriteAllText(filePath, classContent, Encoding.UTF8);

                            Console.WriteLine($"Generated: {filePath}");
                        }
                    }
                }
                Console.WriteLine($"Done");
            }
        }

        private static string FixCase(string input)
        {
            input = input.Replace(":", "_").Replace(".", "_");
            return Regex.Replace(input, @"(?:^|_)([a-z])", m => m.Groups[1].Value.ToUpper());
        }

        private static string FixCase2(string input)
        {
            string[] words = input.Split('_');
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
            return string.Join("", words);
        }

        private static string EntityClassBuilder(string className, string entityId)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("namespace DaemonMC.Entities.VanillaEntities");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className} : Entity");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {className}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            ActorType = \"{entityId}\";");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string BlockClassBuilder(string className, string blockName, Dictionary<string, string> states)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("namespace DaemonMC.Blocks");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className} : Block");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {className}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            Name = \"{blockName}\";\n");

            if (states.Count > 0)
            {
                foreach (var state in states)
                {
                    sb.AppendLine($"            States[\"{state.Key}\"] = {state.Value};");
                }
            }

            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static NbtCompound ReadNBT(Stream data)
        {
            NbtFile file = new NbtFile();
            file.UseVarInt = true;
            file.LoadFromStream(data, NbtCompression.None);
            return (NbtCompound)file.RootTag;
        }
    }
}
