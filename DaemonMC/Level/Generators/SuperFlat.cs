using DaemonMC.Blocks;
using MiNET.LevelDB.Utils;

namespace DaemonMC.Level.Generators
{
    public class SuperFlat
    {
        public byte[] generateChunks()
        {
            using (var stream = new MemoryStream())
            {
                for (int i = 0; i < 1; i++)
                {
                    byte version = 8; //chunk version
                    stream.WriteByte(version);

                    byte storageSize = 1; // Storage size
                    stream.WriteByte(storageSize);

                    for (int a = 0; a < storageSize; a++)
                    {
                        bool isRuntime = true; // Is palette runtime/hash or that second NBT type
                        int bitsPerBlock = 2;
                        byte flag = (byte)(bitsPerBlock << 1 | (isRuntime ? 1 : 0));
                        stream.WriteByte(flag);

                        int blocksPerWord = (int)Math.Floor(32f / bitsPerBlock);
                        uint wordsPerChunk = (uint)Math.Ceiling(4096f / blocksPerWord);

                        // Writing real blocks
                        int position = 0;
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            uint word = 0; // Build the word from block states
                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                int state = 0;

                                int x = position % 16;
                                int y = position / 256;
                                int z = position / 16 % 16;

                                if (x == 3) // add grass
                                {
                                    state = 1;
                                }
                                if (x == 2 || x == 1) // add dirt
                                {
                                    state = 2;
                                }

                                word |= (uint)(state & (1 << bitsPerBlock) - 1) << position % blocksPerWord * bitsPerBlock;
                                position++;
                            }
                            ToDataTypes.WriteUInt32(stream, word);
                        }

                        // Write palette
                        int[] pallette = new int[] { new Air().GetHash(), new GrassBlock().GetHash(), new Dirt().GetHash() };
                        VarInt.WriteSInt32(stream, pallette.Count());// How many block types are in palette?

                        foreach (var block in pallette)
                        {
                            VarInt.WriteSInt32(stream, block);
                        }
                    }
                }
                for (int i = 0; i < 19; i++)//empty chunks again
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
