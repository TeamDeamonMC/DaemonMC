using System.IO.Compression;
using fNbt;
using MiNET.LevelDB;
using MiNET.LevelDB.Utils;

namespace DaemonMC.Tests
{
    [TestClass]
    public class LevelDbTest
    {
        [TestMethod]
        public void InfoLoadTest()
        {
            string LevelName = "My World";
            using (ZipArchive archive = ZipFile.OpenRead($"Worlds/{LevelName}.mcworld"))
            {
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith("level.dat", StringComparison.OrdinalIgnoreCase))
                    {
                        using (Stream stream = entry.Open())
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);

                                memoryStream.Position = 8;

                                var nbt = new NbtFile
                                {
                                    BigEndian = false,
                                    UseVarInt = false
                                };

                                nbt.LoadFromStream(memoryStream, NbtCompression.None);
                                Console.WriteLine(nbt.RootTag);
                                //NbtTag tag = nbt.RootTag;
                                //string LevelName = tag["LevelName"].StringValue;
                                //Console.WriteLine(LevelName);
                            }
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ChunkLoadTest()
        {
            string LevelName = "My World";
            int x = 3;
            int z = -3;
            int count = 0;

            using var db = new Database(new DirectoryInfo($"Worlds/{LevelName}.mcworld"));
            db.Open();

            Console.WriteLine($"Reading chunk x:{x} z:{z} from world:Worlds/{LevelName}.mcworld");
            Console.WriteLine();

            byte[] index = ToDataTypes.GetByteSum(BitConverter.GetBytes(x), BitConverter.GetBytes(z));
            byte[] version = db.Get(ToDataTypes.GetByteSum(index, new byte[] { 0x2c }));
            byte[] dataKey = ToDataTypes.GetByteSum(index, new byte[] { 0x2f, 0 });

            for (int y = -4; y < 20; y++)
            {
                dataKey[^1] = (byte)y;
                byte[] chunk = db.Get(dataKey);

                if (chunk != null)
                {
                    count++;
                    DecodeChunk(chunk);
                }
                else
                {
                    Console.WriteLine($"Empty subchunk (Air?) at x:{x} z:{z} | y:{y}");
                }
            }

            db.Close();
        }

        private void DecodeChunk(ReadOnlySpan<byte> data)
        {
            var reader = new SpanReader(data);

            var version = reader.ReadByte();
            Console.WriteLine($"version: {version}");

            var storageSize = reader.ReadByte();
            Console.WriteLine($"storageSize: {storageSize}");

            var subChunkIndex = reader.ReadByte(); // sub_chunk_index since 1.17.30
            Console.WriteLine($"subChunkIndex: {subChunkIndex}");

            for (int i = 0; i < storageSize; i++)
            {
                var bitsPerBlock = reader.ReadByte() >> 1;
                int blocksPerWord = (int)Math.Floor(32d / bitsPerBlock);
                int wordCount = (int)Math.Ceiling(4096d / blocksPerWord);

                int[] blocks = new int[4096];
                int position = 0;
                for (int wordIdx = 0; wordIdx < wordCount; wordIdx++)
                {
                    uint word = reader.ReadUInt32();

                    for (int block = 0; block < blocksPerWord; block++)
                    {
                        if (position >= 4096) continue;

                        int state = (int)((word >> ((position % blocksPerWord) * bitsPerBlock)) & ((1 << bitsPerBlock) - 1));

                        blocks[position] = state;
                        position++;
                    }
                }

                int paletteSize = reader.ReadInt32();
                Console.WriteLine($"bitsPerBlock: {bitsPerBlock} | paletteSize: {paletteSize}");
                var palette = new List<NbtCompound>();

                for (int j = 0; j < paletteSize; j++)
                {
                    NbtFile file = new NbtFile();
                    file.BigEndian = false;
                    file.UseVarInt = false;
                    var buffer = data.Slice(reader.Position).ToArray();

                    int numberOfBytesRead = (int)file.LoadFromStream(new MemoryStream(buffer), NbtCompression.None);
                    reader.Position += numberOfBytesRead;

                    var rootTag = file.RootTag;
                    if (rootTag != null)
                    {
                        var compound = new NbtCompound("");
                        compound.Add(new NbtString("name", rootTag["name"].StringValue));

                        if (rootTag["states"] is NbtCompound statesTag)
                        {
                            var statesCompound = new NbtCompound("states");
                            foreach (var stateTag in statesTag.Tags)
                            {
                                if (stateTag is NbtInt intState)
                                {
                                    statesCompound.Add(new NbtInt(stateTag.Name, intState.Value));
                                }
                                else if (stateTag is NbtString stringState)
                                {
                                    statesCompound.Add(new NbtString(stateTag.Name, stringState.Value));
                                }
                            }
                            compound.Add(statesCompound);
                        }
                        Console.WriteLine(compound);
                        palette.Add(compound);
                    }
                }

                foreach (var block in blocks)
                {
                    int x = (position >> 8) & 0xF;
                    int y = position & 0xF;
                    int z = (position >> 4) & 0xF;

                    if (block < palette.Count)
                    {
                       // Console.WriteLine($"block x:{x}, y:{y}, z:{z}");
                        var nbt = new NbtFile
                        {
                            BigEndian = false,
                            UseVarInt = false,
                            RootTag = palette[block],
                        };

                        byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);

                        //Console.WriteLine(Fnv1aHash.Hash32(saveToBuffer));
                    }
                    else
                    {
                        Console.WriteLine($"unknown block x:{x}, y:{y}, z:{z} state:{block}");
                    }
                }
                return;
            }
        }
    }
}
