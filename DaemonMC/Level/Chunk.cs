using DaemonMC.Utils;
using DaemonMC.Utils.Text;
using fNbt;
using MiNET.LevelDB.Utils;

namespace DaemonMC.Level
{
    public class Chunk
    {
        public List<SubChunk> chunks = new List<SubChunk>();

        public byte[] networkSerialize(Player player = null)
        {
            using (var stream = new MemoryStream())
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    stream.WriteByte((byte)chunks[i].version);

                    stream.WriteByte((byte)chunks[i].storageSize);

                    for (int a = 0; a < chunks[i].storageSize; a++)
                    {
                        bool isRuntime = chunks[i].isRuntime;
                        int bitsPerBlock = chunks[i].bitsPerBlock;

                        byte flag = (byte)((bitsPerBlock << 1) | (isRuntime ? 1 : 0));
                        stream.WriteByte(flag);

                        int blocksPerWord = (int)Math.Floor(32f / bitsPerBlock);
                        uint wordsPerChunk = (uint)Math.Ceiling(4096f / blocksPerWord);

                        int position = 0;
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            uint word = chunks[i].words[b];
                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                int state = chunks[i].blocks[position];
                                word |= (uint)(state & ((1 << bitsPerBlock) - 1)) << ((position % blocksPerWord) * bitsPerBlock);
                                position++;
                            }
                            ToDataTypes.WriteUInt32(stream, word);
                        }

                        int paletteSize = chunks[i].palette.Count;
                        VarInt.WriteSInt32(stream, paletteSize);

                        for (int v = 0; v < paletteSize; v++)
                        {
                            var nbt = new NbtFile
                            {
                                BigEndian = false,
                                UseVarInt = false,
                                RootTag = chunks[i].palette[v],
                            };

                            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);

                            int blockHash = Fnv1aHash.Hash32(saveToBuffer);
                            VarInt.WriteSInt32(stream, blockHash);
                        }

                        if ((wordsPerChunk * blocksPerWord) != 4096)
                        {
                            Log.error($"LevelDB read error. Wrong block count in subchunk. {wordsPerChunk * blocksPerWord} != 4096");
                            player.Kick($"LevelDB read error. Wrong block count in subchunk. {wordsPerChunk * blocksPerWord} != 4096"); //not sure what to do so. This shouldn't happen. crash client
                        }
                    }
                }

                stream.Write(new byte[256], 0, 256); //not sure about this
                VarInt.WriteSInt32(stream, 0);

                return stream.ToArray();
            }
        }
    }
}
