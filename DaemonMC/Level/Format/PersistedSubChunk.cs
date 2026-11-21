using System.Buffers.Binary;
using fNbt;

namespace DaemonMC.Level.Format
{
    internal sealed class PersistedSubChunk
    {
        private static readonly int[] ValidBitsPerBlock = [0, 1, 2, 3, 4, 5, 6, 8, 16];

        private byte[] header = [];
        private byte[] trailer = [];
        public List<Layer> Layers { get; } = new();

        internal sealed class Layer
        {
            public int BitsPerBlock { get; set; }
            public List<NbtCompound> Palette { get; } = new();
            public ushort[] Blocks { get; } = new ushort[4096];
            public bool Dirty { get; set; }
            internal byte[] Raw { get; set; } = [];

            public int IndexOf(NbtCompound state)
            {
                int hash = StateConverter.Hash(state);
                for (int i = 0; i < Palette.Count; i++)
                {
                    if (StateConverter.Hash(Palette[i]) == hash)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        public static PersistedSubChunk Parse(byte[] data)
        {
            var subChunk = new PersistedSubChunk();
            int pos = 0;
            byte version = data[pos++];
            int storageCount = version switch
            {
                1 => 1,
                8 or 9 => data[pos++],
                _ => throw new NotSupportedException($"sub chunk version {version}")
            };
            if (version == 9)
            {
                pos++; // sub chunk y
            }
            subChunk.header = data[..pos];

            for (int storage = 0; storage < storageCount; storage++)
            {
                int start = pos;
                var layer = new Layer();

                byte flags = data[pos++];
                layer.BitsPerBlock = flags >> 1;
                if ((flags & 1) != 0 || !ValidBitsPerBlock.Contains(layer.BitsPerBlock))
                {
                    throw new NotSupportedException($"storage header 0x{flags:x2}");
                }

                if (layer.BitsPerBlock != 0)
                {
                    int bits = layer.BitsPerBlock;
                    int blocksPerWord = 32 / bits;
                    int wordCount = (4096 + blocksPerWord - 1) / blocksPerWord;
                    uint mask = (1u << bits) - 1;
                    for (int word = 0; word < wordCount; word++)
                    {
                        uint value = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(pos, 4));
                        pos += 4;
                        for (int i = 0; i < blocksPerWord; i++)
                        {
                            int index = word * blocksPerWord + i;
                            if (index >= 4096) break;
                            layer.Blocks[index] = (ushort)((value >> (i * bits)) & mask);
                        }
                    }
                }

                int paletteSize = 1;

                if (layer.BitsPerBlock != 0 || data[pos] != (byte)NbtTagType.Compound)
                {
                    paletteSize = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(pos, 4));
                    pos += 4;
                }

                for (int i = 0; i < paletteSize; i++)
                {
                    var nbt = new NbtFile
                    {
                        BigEndian = false,
                        UseVarInt = false
                    };
                    pos += (int)nbt.LoadFromStream(new MemoryStream(data, pos, data.Length - pos, false), NbtCompression.None);
                    layer.Palette.Add(nbt.RootTag as NbtCompound ?? throw new InvalidDataException("palette entry is not a compound"));
                }

                layer.Raw = data[start..pos];
                subChunk.Layers.Add(layer);
            }

            subChunk.trailer = data[pos..];
            return subChunk;
        }

        public byte[] Serialize()
        {
            using var output = new MemoryStream();
            output.Write(header);
            foreach (var layer in Layers)
            {
                if (layer.Dirty)
                {
                    WriteLayer(output, layer);
                }
                else
                {
                    output.Write(layer.Raw);
                }
            }
            output.Write(trailer);
            return output.ToArray();
        }

        private static void WriteLayer(MemoryStream output, Layer layer)
        {
            int bits;
            if (layer.Palette.Count == 1 && layer.BitsPerBlock == 0)
            {
                bits = 0;
            }
            else
            {
                bits = ValidBitsPerBlock.First(b => b != 0 && b >= layer.BitsPerBlock && layer.Palette.Count <= 1 << b);
            }

            output.WriteByte((byte)(bits << 1));

            Span<byte> buffer = stackalloc byte[4];
            if (bits != 0)
            {
                int blocksPerWord = 32 / bits;
                int wordCount = (4096 + blocksPerWord - 1) / blocksPerWord;
                for (int word = 0; word < wordCount; word++)
                {
                    uint value = 0;
                    for (int i = 0; i < blocksPerWord; i++)
                    {
                        int index = word * blocksPerWord + i;
                        if (index >= 4096) break;
                        value |= (uint)layer.Blocks[index] << (i * bits);
                    }
                    BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
                    output.Write(buffer);
                }

                BinaryPrimitives.WriteInt32LittleEndian(buffer, layer.Palette.Count);
                output.Write(buffer);
            }

            foreach (var state in layer.Palette)
            {
                var nbt = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = false,
                    RootTag = state
                };
                output.Write(nbt.SaveToBuffer(NbtCompression.None));
            }
        }
    }
}
