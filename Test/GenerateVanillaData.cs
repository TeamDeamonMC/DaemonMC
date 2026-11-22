using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DaemonMC.Utils;
using fNbt;

namespace Test
{
    [TestClass]
    public class GenerateVanillaData
    {
        [TestMethod]
        public void Blocks()
        {
            var json = File.ReadAllText("block_properties_table.json"); //https://github.com/pmmp/BedrockData/blob/master/block_properties_table.json

            var blockProperties = JsonSerializer.Deserialize<Dictionary<string, BlockProperties>>(json);

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

                    BlockProperties props = null;

                    if (blockProperties.TryGetValue(blockName, out var foundProps))
                    {
                        props = foundProps;
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

                    string classContent = BlockClassBuilder(className, blockName, states, props);

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
			
            var collidingNames = doc.RootElement.EnumerateObject().GroupBy(i => FixCase(i.Name.Split(':')[1]), StringComparer.OrdinalIgnoreCase).Where(g => g.Count() > 1).SelectMany(g => g.Select(i => i.Name)).ToHashSet();

            foreach (var item in doc.RootElement.EnumerateObject())
            {
                string name = item.Name;

                var obj = item.Value;

                int id = obj.GetProperty("runtime_id").GetInt32();
                int version = obj.GetProperty("version").GetInt32();
                bool componentBased = obj.GetProperty("component_based").GetBoolean();
                string componentNbt = obj.TryGetProperty("component_nbt", out JsonElement value) ? value.GetString() : "";

                string className = FixCase(name.Split(':')[1]);

                if (collidingNames.Contains(name) && id < 256)
                {
                    className += "Block";
                }

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
            ComponentData = {JsonSerializer.Serialize(componentNbt)};
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

        [TestMethod]
        public void Sounds()
        {
            string json = File.ReadAllText("level_sound_id_map.json"); //https://github.com/pmmp/BedrockData/blob/master/level_sound_id_map.json
            var sounds = JsonSerializer.Deserialize<Dictionary<string, int>>(json)!;

            var sb = new StringBuilder();

            sb.AppendLine("public Dictionary<int, string> Sounds = new Dictionary<int, string>()");
            sb.AppendLine("{");

            foreach (var kv in sounds.OrderBy(x => x.Value))
            {
                sb.AppendLine($"    {{ {kv.Value}, \"{kv.Key}\" }},");
                Console.WriteLine(kv.Key);
            }

            sb.AppendLine("};");

            File.WriteAllText("Sounds.cs", sb.ToString());
            Console.WriteLine("Done");
        }

        [TestMethod]
        public void Biomes()
        {
            //https://github.com/pmmp/BedrockData/blob/master/biome_definitions.json
            var definitions = JsonDocument.Parse(File.ReadAllText("biome_definitions.json")).RootElement;
            //https://github.com/pmmp/BedrockData/blob/master/biome_id_map.json
            var ids = JsonSerializer.Deserialize<Dictionary<string, int>>(File.ReadAllText("biome_id_map.json"))!;

            var inv = System.Globalization.CultureInfo.InvariantCulture;
            string F(JsonElement e) => e.GetSingle().ToString("R", inv) + "f";

            var sb = new StringBuilder();
            sb.AppendLine("// Generated by Test class. Do not edit by hand.");
            sb.AppendLine("namespace DaemonMC.Biomes");
            sb.AppendLine("{");
            sb.AppendLine("    public static class VanillaBiomes");
            sb.AppendLine("    {");
            sb.AppendLine("        public static List<Biome> Create() => new List<Biome>()");
            sb.AppendLine("        {");

            var entries = new List<(int id, string line)>();
            foreach (var biome in definitions.EnumerateObject())
            {
                string name = biome.Name.Replace("minecraft:", "");
                if (!ids.TryGetValue(name, out int id))
                {
                    Console.WriteLine($"Skipped {name}: no numeric id in biome_id_map.json");
                    continue;
                }

                var d = biome.Value;
                var c = d.GetProperty("mapWaterColour");
                uint argb = (uint)c.GetProperty("a").GetInt32() << 24 | (uint)c.GetProperty("r").GetInt32() << 16 | (uint)c.GetProperty("g").GetInt32() << 8 | (uint)c.GetProperty("b").GetInt32();
                var tags = d.TryGetProperty("tags", out var t) ? t.EnumerateArray().Select(x => $"\"{x.GetString()}\"") : [];

                entries.Add((id, $"            new Biome() {{ BiomeName = \"{name}\", Id = {id}, BiomeData = new BiomeDefinitionData() {{ " +
                    $"Temperature = {F(d.GetProperty("temperature"))}, Downfall = {F(d.GetProperty("downfall"))}, FoliageSnow = {F(d.GetProperty("foliageSnow"))}, " +
                    $"Depth = {F(d.GetProperty("depth"))}, Scale = {F(d.GetProperty("scale"))}, WaterColor = unchecked((int)0x{argb:X8}), " +
                    $"Rain = {(d.GetProperty("rain").GetBoolean() ? "true" : "false")}, Tags = [{string.Join(", ", tags)}] }} }},"));
            }

            foreach (var name in ids.Keys.Where(n => !definitions.TryGetProperty($"minecraft:{n}", out _)))
            {
                Console.WriteLine($"Warning: {name} has an id but no definition");
            }

            foreach (var entry in entries.OrderBy(e => e.id))
            {
                sb.AppendLine(entry.line);
            }

            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText("VanillaBiomes.cs", sb.ToString(), Encoding.UTF8);
            Console.WriteLine($"Generated: VanillaBiomes.cs ({entries.Count} biomes)");
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

        private static string BlockClassBuilder(string className, string blockName, Dictionary<string, string> states, BlockProperties props)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("namespace DaemonMC.Blocks");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {className} : Block");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {className}()");
            sb.AppendLine("        {");
            sb.AppendLine($"            Name = \"{blockName}\";\n");

            if (props != null)
            {
                sb.AppendLine($"            BlastResistance = {props.blastResistance};");
                sb.AppendLine($"            Brightness = {props.brightness};");
                sb.AppendLine($"            FlameEncouragement = {props.flameEncouragement};");
                sb.AppendLine($"            Flammability = {props.flammability};");
                sb.AppendLine($"            Friction = {props.friction};");
                sb.AppendLine($"            Hardness = {props.hardness};");
                sb.AppendLine($"            Opacity = {props.opacity};");
            }

            if (states.Count > 0)
            {
                sb.AppendLine();
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

        public class BlockProperties
        {
            public double blastResistance { get; set; }
            public double brightness { get; set; }
            public int flameEncouragement { get; set; }
            public int flammability { get; set; }
            public double friction { get; set; }
            public double hardness { get; set; }
            public double opacity { get; set; }
        }
    }
}
