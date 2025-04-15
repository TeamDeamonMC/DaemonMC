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

                        subChunk.Blocks[position] = (byte)((word >> ((position % blocksPerWord) * subChunk.BitsPerBlock)) & ((1 << subChunk.BitsPerBlock) - 1));

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
