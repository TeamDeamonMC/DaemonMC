using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Test
{
    [TestClass]
    public class Blocks
    {
        [TestMethod]
        public void GenerateStates()
        {
            string json = File.ReadAllText("block_states.json"); //https://github.com/SerenityJS/bedrock-data/blob/main/data/1.21.90/block_states.json
            var definitions = JsonSerializer.Deserialize<List<BlockStateDefinition>>(json);

            var sb = new StringBuilder();

            sb.AppendLine("public static readonly Dictionary<string, object[]> States = new()");
            sb.AppendLine("{");

            foreach (var def in definitions)
            {
                string valuesString;

                switch (def.Type)
                {
                    case "bool":
                        valuesString = string.Join(", ", def.Values.Select(v => v.GetBoolean() ? "(byte)1" : "(byte)0"));
                        break;

                    case "int":
                        valuesString = string.Join(", ", def.Values.Select(v => v.GetInt32().ToString()));
                        break;

                    case "string":
                        valuesString = string.Join(", ", def.Values.Select(v => $"\"{v.GetString()}\""));
                        break;

                    default:
                        throw new Exception($"unknown type {def.Type}");
                }

                sb.AppendLine($"    {{ \"{def.Identifier}\", new object[] {{ {valuesString} }} }},");
            }

            sb.AppendLine("};");

            Console.WriteLine(sb.ToString());
        }
    }

    public class BlockStateDefinition
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("values")]
        public JsonElement[] Values { get; set; }
    }
}
