using DaemonMC.Blocks;
using DaemonMC.Utils;
using fNbt;

namespace Test
{
    [TestClass]
    public class Nbt
    {
        [TestMethod]
        public void DecodeTest()
        {
            using (var stream = File.OpenRead("item_components.nbt"))
            {
                while (stream.Position < stream.Length)
                {
                    var compound = ReadNBT(stream, NbtCompression.GZip);
                    Console.WriteLine(compound);
                }
            }
        }

        [TestMethod]
        public void BlockNbtTest()
        {
            var block = new GrassBlock();

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = block.GetState(),
            };

            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);

            Console.WriteLine($"block nbt: {nbt}");
            Console.WriteLine($"block nbt buffer: {BitConverter.ToString(saveToBuffer)}");
            Console.WriteLine($"block hash ID: {Fnv1aHash.Hash32(saveToBuffer)}");
        }

        [TestMethod]
        public void EmptyCompound()
        {
            NbtFile file = new NbtFile(new NbtCompound(""));

            file.BigEndian = false;
            file.UseVarInt = true;

            byte[] serializedTag = file.SaveToBuffer(NbtCompression.None);

            Console.WriteLine($"nbt buffer: {BitConverter.ToString(serializedTag)}");
        }

        [TestMethod]
        public void DecodeItemComponentNBT()
        {
            var raw = "CgAACgoAY29tcG9uZW50cwoPAGl0ZW1fcHJvcGVydGllcwEOAGFsbG93X29mZl9oYW5kAAEXAGNhbl9kZXN0cm95X2luX2NyZWF0aXZlAQMRAGNyZWF0aXZlX2NhdGVnb3J5AgAAAAgOAGNyZWF0aXZlX2dyb3VwAAADBgBkYW1hZ2UAAAAACBAAZW5jaGFudGFibGVfc2xvdAQAbm9uZQMRAGVuY2hhbnRhYmxlX3ZhbHVlAAAAAAEEAGZvaWwAAwsAZnJhbWVfY291bnQBAAAAAQ0AaGFuZF9lcXVpcHBlZAABEgBoaWRkZW5faW5fY29tbWFuZHMCAQ4AbGlxdWlkX2NsaXBwZWQAAw4AbWF4X3N0YWNrX3NpemVAAAAACg4AbWluZWNyYWZ0Omljb24KCAB0ZXh0dXJlcwgHAGRlZmF1bHQFAGFwcGxlAAAFDABtaW5pbmdfc3BlZWQAAIA/AQ4Ac2hvdWxkX2Rlc3Bhd24BAQ8Ac3RhY2tlZF9ieV9kYXRhAAMNAHVzZV9hbmltYXRpb24BAAAAAwwAdXNlX2R1cmF0aW9uIAAAAAAJCQBpdGVtX3RhZ3MIAQAAABEAbWluZWNyYWZ0OmlzX2Zvb2QKFgBtaW5lY3JhZnQ6ZGlzcGxheV9uYW1lCAUAdmFsdWUPAGl0ZW0uYXBwbGUubmFtZQAKDgBtaW5lY3JhZnQ6Zm9vZAEOAGNhbl9hbHdheXNfZWF0AAMJAG51dHJpdGlvbgQAAAAFEwBzYXR1cmF0aW9uX21vZGlmaWVympmZPgoRAHVzaW5nX2NvbnZlcnRzX3RvAAAKDgBtaW5lY3JhZnQ6dGFncwkEAHRhZ3MIAQAAABEAbWluZWNyYWZ0OmlzX2Zvb2QAChcAbWluZWNyYWZ0OnVzZV9hbmltYXRpb24IBQB2YWx1ZQMAZWF0AAoXAG1pbmVjcmFmdDp1c2VfbW9kaWZpZXJzAQ8AZW1pdF92aWJyYXRpb25zAQURAG1vdmVtZW50X21vZGlmaWVyMzOzPggLAHN0YXJ0X3VzaW5nBgBhbHdheXMFDAB1c2VfZHVyYXRpb27NzMw/AAAA";
            byte[] data = Convert.FromBase64String(raw);

            var file = new NbtFile();
            file.BigEndian = false;
            file.LoadFromBuffer(data, 0, data.Length, NbtCompression.None);

            Console.WriteLine(file.RootTag);
        }

        public static NbtCompound ReadNBT(Stream data, NbtCompression compression = NbtCompression.None)
        {
            NbtFile file = new NbtFile();
            file.UseVarInt = false;
            file.BigEndian = true;
            file.LoadFromStream(data, compression);
            return (NbtCompound)file.RootTag;
        }
    }
}
