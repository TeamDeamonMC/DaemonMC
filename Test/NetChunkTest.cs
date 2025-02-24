using DaemonMC.Level;
using MiNET.LevelDB.Utils;

namespace DaemonMC
{
    [TestClass]
    public class NetChunkTest
    {
        [TestMethod]
        public void DeserializeChunk()
        {
            //var chunkData = generateChunks();
            //var chunkData = testchunk.flat;
            var level = new World("My World");
            var chunk = level.GetChunk(0, 0);
            var stream = new MemoryStream(chunk.NetworkSerialize());
            {
                for (int i = 0; i < chunk.Chunks.Count; i++)
                {
                    Console.WriteLine($"reading sunchunk: {i}");

                    int version = stream.ReadByte();
                    Console.WriteLine($"version: {version}");

                    int storageSize = stream.ReadByte();
                    Console.WriteLine($"storageSize: {storageSize}");

                    for (int a = 0; a < storageSize; a++)
                    {
                        var flag = stream.ReadByte();

                        bool isRuntime = (flag & 1) != 0;
                        Console.WriteLine($"isRuntime: {isRuntime}");

                        int bitsPerBlock = flag >> 1;
                        Console.WriteLine($"bitsPerBlock: {bitsPerBlock}");

                        int blocksPerWord = (int)Math.Floor(32f / bitsPerBlock);
                        Console.WriteLine($"blocksPerWord: {blocksPerWord}");

                        uint wordsPerChunk = (uint)Math.Ceiling(4096f / blocksPerWord);
                        Console.WriteLine($"wordsPerChunk: {wordsPerChunk}");

                        uint[] blocks = new uint[wordsPerChunk];
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            blocks[b] = ToDataTypes.ReadUInt32(stream);
                        }

                        var paletteSize = VarInt.ReadSInt32(stream);
                        Console.WriteLine($"paletteSize: {paletteSize}");

                        int[] palette = new int[paletteSize];

                        for (int v = 0; v < paletteSize; v++)
                        {
                            palette[v] = VarInt.ReadSInt32(stream);
                            Console.WriteLine($"itemRuntimeId: {palette[v]}");
                        }

                        long afterPos = stream.Position;

                        int position = 0;
                        for (int w = 0; w < wordsPerChunk; w++)
                        {
                            var word = blocks[w];

                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                var state = (word >> ((position % blocksPerWord) * bitsPerBlock)) & ((1 << bitsPerBlock) - 1);

                                int x = (position >> 8) & 0xf;
                                int y = position & 0xf;
                                int z = (position >> 4) & 0xf;

                                if (state < palette.Length)
                                {
                                    var blockValue = palette[state];
                                    Console.WriteLine($"Block at ({x}, {y}, {z}): {blockValue}");
                                }
                                else
                                {
                                    Console.WriteLine($"state error: {state}");
                                }
                                position++;
                            }
                        }
                        stream.Position = afterPos;
                    }
                }

                stream.Read(new byte[256], 0, 256);

                int borderBlock = (byte)VarInt.ReadSInt32(stream);
                Console.WriteLine($"borderBlock {borderBlock}");

                if (stream.Position < stream.Length - 1)
                {
                    Console.WriteLine($"Need to read more {stream.Length - stream.Position} bytes");
                }
            }
        }


        public static byte[] generateChunks()
        {
            using (var stream = new MemoryStream())
            {
                for (int i = 0; i < 3; i++)//lets write some empty chunks
                {
                    stream.WriteByte(8);//chunk version
                    stream.WriteByte(0);//storage size
                }
                for (int i = 0; i < 1; i++)
                {
                    Console.WriteLine($"writing sunchunk: {i}");

                    byte version = 8; //chunk version
                    stream.WriteByte(version);

                    byte storageSize = 1; // Storage size
                    stream.WriteByte(storageSize);

                    for (int a = 0; a < storageSize; a++)
                    {
                        bool isRuntime = true; // Is palette runtime/hash or that second NBT type
                        int bitsPerBlock = 2;
                        byte flag = (byte)((bitsPerBlock << 1) | (isRuntime ? 1 : 0));
                        stream.WriteByte(flag);

                        int blocksPerWord = (int)Math.Floor(32f / bitsPerBlock);
                        uint wordsPerChunk = (uint)Math.Ceiling(4096f / blocksPerWord);

                        // Write palette
                        int position = 0;
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            uint word = 0; // Build the word from block states
                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                int state = 0; // Example logic: replace with actual state
                                word |= (uint)(state & ((1 << bitsPerBlock) - 1)) << ((position % blocksPerWord) * bitsPerBlock);
                                position++;
                            }
                            ToDataTypes.WriteUInt32(stream, word);
                        }

                        // Writing real blocks
                        int paletteSize = 1; // How many blocks is in palette?
                        VarInt.WriteSInt32(stream, paletteSize);

                        for (int v = 0; v < paletteSize; v++)
                        {
                            int itemRuntimeId = 10539; // grass block runtime id
                            VarInt.WriteSInt32(stream, itemRuntimeId);
                        }
                    }
                }
                for (int i = 0; i < 16; i++)//empty chunks again
                {
                    stream.WriteByte(8);//version
                    stream.WriteByte(0);//storage size
                }

                stream.Write(new byte[256], 0, 256); //not sure about this
                VarInt.WriteSInt32(stream, 0);

                return stream.ToArray();
            }
        }
    }
}
