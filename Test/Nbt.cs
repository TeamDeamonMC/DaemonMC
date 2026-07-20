using DaemonMC.Blocks;
using DaemonMC.Utils;
using DaemonMC.Utils.Text;
using fNbt;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class Nbt
    {
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
    }
}
