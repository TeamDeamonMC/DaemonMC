using DaemonMC.Level.Format;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using fNbt;
using MiNET.LevelDB.Utils;

namespace DaemonMC.Level
{
    public class Chunk
    {
        public List<SubChunk> Chunks = new List<SubChunk>();

        public byte[] NetworkSerialize(Player ?player = null)
        {
            int protocol = Info.ProtocolVersion.Last();

            if (player != null)
            {
                protocol = RakSessionManager.getSession(player.ep).protocolVersion;
            }

            using (var stream = new MemoryStream())
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    stream.WriteByte((byte)Chunks[i].Version);

                    stream.WriteByte((byte)Chunks[i].StorageSize);

                    for (int a = 0; a < Chunks[i].StorageSize; a++)
                    {
                        bool isRuntime = Chunks[i].IsRuntime;
                        int bitsPerBlock = Chunks[i].BitsPerBlock;
                        byte flag = (byte)((bitsPerBlock << 1) | (isRuntime ? 1 : 0));
                        stream.WriteByte(flag);

                        int blocksPerWord = (int)Math.Floor(32f / bitsPerBlock);
                        uint wordsPerChunk = (uint)Math.Ceiling(4096f / blocksPerWord);

                        int position = 0;
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            uint word = 0;
                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                if (position >= 4096) continue;
                                int state = Chunks[i].Blocks[position];
                                word |= (uint)(state & ((1 << bitsPerBlock) - 1)) << ((position % blocksPerWord) * bitsPerBlock);
                                position++;
                            }
                            ToDataTypes.WriteUInt32(stream, word);
                        }

                        int paletteSize = Chunks[i].Palette.Count;
                        VarInt.WriteSInt32(stream, paletteSize);

                        var blockPalette = StateConverter.process(Chunks[i].Palette, protocol);

                        for (int v = 0; v < paletteSize; v++)
                        {
                            var nbt = new NbtFile
                            {
                                BigEndian = false,
                                UseVarInt = false,
                                RootTag = blockPalette[v],
                            };

                            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);

                            int blockHash = Fnv1aHash.Hash32(saveToBuffer);
                            VarInt.WriteSInt32(stream, blockHash);
                        }
                    }
                }

                for (int i = 0; i < Chunks.Count; i++)
                {
                    for (int a = 0; a < Chunks[i].StorageSize; a++)
                    {
                        bool isRuntime = Chunks[i].IsRuntime;
                        int bitsPerBlock = Chunks[i].BitsPerBlock;
                        byte flag = (byte)((bitsPerBlock << 1) | (isRuntime ? 1 : 0));
                        stream.WriteByte(flag);

                        int blocksPerWord = (int)Math.Floor(32f / bitsPerBlock);
                        uint wordsPerChunk = (uint)Math.Ceiling(4096f / blocksPerWord);

                        int position = 0;
                        for (int b = 0; b < wordsPerChunk; b++)
                        {
                            uint word = 0;
                            for (int block = 0; block < blocksPerWord; block++)
                            {
                                if (position >= 4096) continue;
                                int state = Chunks[i].Biomes[position];
                                word |= (uint)(state & ((1 << bitsPerBlock) - 1)) << ((position % blocksPerWord) * bitsPerBlock);
                                position++;
                            }
                            ToDataTypes.WriteUInt32(stream, word);
                        }

                        int paletteSize = Chunks[i].BiomePalette.Count;
                        VarInt.WriteSInt32(stream, paletteSize);

                        for (int v = 0; v < paletteSize; v++)
                        {
                            VarInt.WriteSInt32(stream, Chunks[i].BiomePalette[v]);
                        }
                    }
                }

                stream.WriteByte(0); //border blocks
                VarInt.WriteSInt32(stream, 0);

                return stream.ToArray();
            }
        }
    }
}
