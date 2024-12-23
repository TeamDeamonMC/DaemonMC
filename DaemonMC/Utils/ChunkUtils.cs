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

            subChunk.storageSize = reader.ReadByte();

            reader.ReadByte(); // sub_chunk_index since 1.17.30 TODO

            for (int i = 0; i < subChunk.storageSize; i++) //read real blocks
            {
                subChunk.bitsPerBlock = reader.ReadByte() >> 1;
                int blocksPerWord = (int)Math.Floor(32d / subChunk.bitsPerBlock);
                int wordCount = (int)Math.Ceiling(4096d / blocksPerWord);

                int position = 0;
                for (int wordIdx = 0; wordIdx < wordCount; wordIdx++)
                {
                    uint word = reader.ReadUInt32();
                    subChunk.words[wordIdx] = word;
                    for (int block = 0; block < blocksPerWord; block++)
                    {
                        if (position >= 4096) continue;

                        subChunk.blocks[position] = (byte)((word >> ((position % blocksPerWord) * subChunk.bitsPerBlock)) & ((1 << subChunk.bitsPerBlock) - 1));

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
                                else if (stateTag is NbtByte byteState)
                                {
                                    statesCompound.Add(new NbtByte(stateTag.Name, byteState.Value));
                                }
                            }
                            compound.Add(statesCompound);
                        }

                        subChunk.palette.Add(compound);
                    }
                }
            }
            return subChunk;
        }

        public static List<(int x, int z)> GetSequence(int radius, int x, int z)
        {
            var positions = new List<(int x, int z)>();
            var visited = new HashSet<(int x, int z)>();

            positions.Add((0, 0));
            visited.Add((0, 0));

            var queue = new Queue<(int x, int z)>();
            queue.Enqueue((0, 0));

            while (queue.Count > 0)
            {
                (x, z) = queue.Dequeue();

                foreach (var (nx, nz) in GetNear(x, z))
                {
                    if (!visited.Contains((nx, nz)) && Math.Sqrt(nx * nx + nz * nz) <= radius)
                    {
                        visited.Add((nx, nz));
                        positions.Add((nx, nz));
                        queue.Enqueue((nx, nz));
                    }
                }
            }

            return positions;
        }

        private static List<(int x, int z)> GetNear(int x, int z)
        {
            return new List<(int, int)>
            {
                (x + 1, z),
                (x - 1, z),
                (x, z + 1),
                (x, z - 1)
            };
        }
    }
}
