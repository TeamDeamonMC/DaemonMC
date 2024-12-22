using MiNET.LevelDB.Utils;

namespace DaemonMC.Level
{
    public class testchunk
    {
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

                        // Writing real blocks
                        int position = 0;
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            uint word = 0; // Build the word from block states
                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                int state = 0;
                                word |= (uint)(state & ((1 << bitsPerBlock) - 1)) << ((position % blocksPerWord) * bitsPerBlock);
                                position++;
                            }
                            ToDataTypes.WriteUInt32(stream, word);
                        }

                        // Write palette
                        int paletteSize = 1; // How many block types are in palette?
                        VarInt.WriteSInt32(stream, paletteSize);

                        for (int v = 0; v < paletteSize; v++)
                        {
                            int itemRuntimeId = -567203660; // grass hash runtime id (block type)
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
