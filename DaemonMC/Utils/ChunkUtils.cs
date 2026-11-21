using DaemonMC.Blocks;
using DaemonMC.Level;
using fNbt;
using MiNET.LevelDB.Utils;

namespace DaemonMC.Utils
{
    public class ChunkUtils
    {
        public static SubChunk DecodeSubChunk(ReadOnlySpan<byte> data)
        {
            var subChunk = new SubChunk();
            var reader = new SpanReader(data);

            reader.ReadByte();//Todo support new format

            subChunk.StorageSize = reader.ReadByte();

            reader.ReadByte(); // sub_chunk_index since 1.17.30 TODO

            for (int i = 0; i < subChunk.StorageSize; i++) //read real blocks
            {
                var blocks = subChunk.Blocks;
                subChunk.BitsPerBlock = reader.ReadByte() >> 1;
                int blocksPerWord = (int)Math.Floor(32d / subChunk.BitsPerBlock);
                int wordCount = (int)Math.Ceiling(4096d / blocksPerWord);

                int position = 0;
                for (int wordIdx = 0; wordIdx < wordCount; wordIdx++)
                {
                    uint word = reader.ReadUInt32();
                    for (int block = 0; block < blocksPerWord; block++)
                    {
                        if (position >= 4096) continue;

                        blocks[position] =(byte)((word >> ((position % blocksPerWord) * subChunk.BitsPerBlock)) & ((1 << subChunk.BitsPerBlock) - 1));

                        position++;
                    }
                }

                int paletteSize = reader.ReadInt32();

                for (int j = 0; j < paletteSize; j++) //read block type palette
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
                        var compound = new NbtCompound("")
                        {
                            new NbtString("name", rootTag["name"].StringValue)
                        };

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
                                else if (stateTag is NbtByte byteState)
                                {
                                    statesCompound.Add(new NbtByte(stateTag.Name, byteState.Value));
                                }
                            }
                            compound.Add(statesCompound);
                        }

                        subChunk.Palette.Add(compound);
                    }
                }
                return subChunk; //todo fix and remove here. this usually works but will not always. multiple storages thorw error
            }
            return subChunk; //keep this
        }

        private static readonly int[] ValidBitsPerBlock = [1, 2, 3, 4, 5, 6, 8];

        public static int GetBitsPerBlock(int paletteSize)
        {
            foreach (int bits in ValidBitsPerBlock)
            {
                if (paletteSize <= (1 << bits))
                {
                    return bits;
                }
            }
            return ValidBitsPerBlock[^1];
        }

        public static void WritePalettedStorage(Stream stream, byte[] indices, IReadOnlyList<int> values)
        {
            if (values.Count <= 1)
            {
                stream.WriteByte(1); //(0 << 1) | 1
                VarInt.WriteSInt32(stream, values.Count == 1 ? values[0] : 0); //palette size for 0 bits is 0 ofc
                return;
            }

            int bitsPerBlock = GetBitsPerBlock(values.Count);
            stream.WriteByte((byte)((bitsPerBlock << 1) | 1));

            int blocksPerWord = 32 / bitsPerBlock;
            int wordCount = (4096 + blocksPerWord - 1) / blocksPerWord;
            uint mask = (1u << bitsPerBlock) - 1;

            int position = 0;
            for (int w = 0; w < wordCount; w++)
            {
                uint word = 0;
                for (int b = 0; b < blocksPerWord && position < 4096; b++, position++)
                {
                    word |= (indices[position] & mask) << (b * bitsPerBlock);
                }
                ToDataTypes.WriteUInt32(stream, word);
            }

            VarInt.WriteSInt32(stream, values.Count);
            foreach (int value in values)
            {
                VarInt.WriteSInt32(stream, value);
            }
        }

        public static SubChunk CreateEmptySubChunk()
        {
            var subChunk = new SubChunk
            {
                StorageSize = 1,
                BitsPerBlock = 1,
            };
            subChunk.Palette.Add(new Air().GetState());
            return subChunk;
        }

        public static List<(int x, int z)> GetSequence(int radius, int centerX, int centerZ)
        {
            var positions = new List<(int x, int z)>();

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dz = -radius; dz <= radius; dz++)
                {
                    if (Math.Sqrt(dx * dx + dz * dz) <= radius)
                    {
                        positions.Add((centerX + dx, centerZ + dz));
                    }
                }
            }

            return positions;
        }
    }
}
