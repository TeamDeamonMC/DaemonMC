using DaemonMC.Biomes;
using DaemonMC.Level.Format;
using DaemonMC.Network;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using fNbt;
using MiNET.LevelDB.Utils;

namespace DaemonMC.Level
{
    public class Chunk
    {
        public List<SubChunk> Chunks = new List<SubChunk>();

        private const int NetworkSubChunkVersion = 9;

        private static int GetProtocol(Player? player)
        {
            if (player != null)
            {
                return RakSessionManager.getSession(player.ep).protocolVersion;
            }
            return Info.ProtocolVersion.Last();
        }

        private static bool[] GetAirMask(SubChunk subChunk)
        {
            var mask = new bool[subChunk.Palette.Count];
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] = subChunk.Palette[i]["name"]?.StringValue == "minecraft:air";
            }
            return mask;
        }

        public bool IsSubChunkAir(int index)
        {
            if (index < 0 || index >= Chunks.Count) return true;

            var subChunk = Chunks[index];
            if (subChunk.StorageSize == 0 || subChunk.Palette.Count == 0) return true;

            var airMask = GetAirMask(subChunk);
            if (Array.TrueForAll(airMask, a => a)) return true;

            var blocks = subChunk.Blocks;
            for (int i = 0; i < 4096; i++)
            {
                int state = blocks[i];
                if (state < airMask.Length && !airMask[state]) return false;
            }
            return true;
        }

        public int GetHighestNonAirSubChunk()
        {
            for (int i = Chunks.Count - 1; i >= 0; i--)
            {
                if (!IsSubChunkAir(i)) return i;
            }
            return -1;
        }

        public int[] GetColumnHeights()
        {
            var heights = new int[256];
            Array.Fill(heights, -1);
            int remaining = 256;

            for (int i = Chunks.Count - 1; i >= 0 && remaining > 0; i--)
            {
                if (IsSubChunkAir(i)) continue;

                var subChunk = Chunks[i];
                var airMask = GetAirMask(subChunk);
                var blocks = subChunk.Blocks;

                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        int column = (z << 4) | x;
                        if (heights[column] != -1) continue;

                        for (int y = 15; y >= 0; y--)
                        {
                            int state = blocks[(x << 8) | (z << 4) | y];
                            if (state < airMask.Length && !airMask[state])
                            {
                                heights[column] = i * 16 + y;
                                remaining--;
                                break;
                            }
                        }
                    }
                }
            }
            return heights;
        }

        public static HeightMapDataType BuildSubChunkHeightMap(int[] columnHeights, int index, out sbyte[] heightMap)
        {
            heightMap = new sbyte[256];
            bool allTooLow = true;
            bool allTooHigh = true;
            int bottom = index * 16;

            for (int i = 0; i < 256; i++)
            {
                int h = columnHeights[i];
                sbyte value;
                if (h < bottom)
                {
                    value = -1;
                    allTooHigh = false;
                }
                else if (h > bottom + 15)
                {
                    value = 16;
                    allTooLow = false;
                }
                else
                {
                    value = (sbyte)(h - bottom);
                    allTooLow = false;
                    allTooHigh = false;
                }
                heightMap[i] = value;
            }

            if (allTooLow) { heightMap = []; return HeightMapDataType.AllTooLow; }
            if (allTooHigh) { heightMap = []; return HeightMapDataType.AllTooHigh; }
            return HeightMapDataType.HasData;
        }

        private static void WritePalettedStorage(Stream stream, byte[] indices, IReadOnlyList<int> values)
        {
            if (values.Count <= 1)
            {
                stream.WriteByte(1); //(0 << 1) | 1
                VarInt.WriteSInt32(stream, values.Count == 1 ? values[0] : 0); //palette size for 0 bits is 0 ofc
                return;
            }

            int bitsPerBlock = ChunkUtils.GetBitsPerBlock(values.Count);
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

        public byte[] NetworkSerializeBiomes(Dimension dimension)
        {
            int defaultBiome = BiomeManager.GetBiomeId("plains");

            using var stream = new MemoryStream();
            byte[]? previous = null;

            for (int i = 0; i < dimension.SubChunkCount(); i++)
            {
                byte[] current;
                if (i < Chunks.Count && Chunks[i].BiomePalette.Count > 0)
                {
                    using var biomeStream = new MemoryStream();
                    WritePalettedStorage(biomeStream, Chunks[i].Biomes, Chunks[i].BiomePalette);
                    current = biomeStream.ToArray();
                }
                else if (previous != null)
                {
                    current = previous;
                }
                else
                {
                    using var biomeStream = new MemoryStream();
                    WritePalettedStorage(biomeStream, Chunks.Count > 0 ? Chunks[0].Biomes : new byte[4096], [defaultBiome]);
                    current = biomeStream.ToArray();
                }

                if (previous != null && current.AsSpan().SequenceEqual(previous))
                {
                    stream.WriteByte((127 << 1) | 1); //same as previous sub chunk
                }
                else
                {
                    stream.Write(current);
                }
                previous = current;
            }

            stream.WriteByte(0); // boder blocks
            return stream.ToArray();
        }

        public byte[] NetworkSerializeSubChunk(int index, int absoluteY, Player? player = null)
        {
            int protocol = GetProtocol(player);
            var subChunk = Chunks[index];

            using var stream = new MemoryStream();
            stream.WriteByte(NetworkSubChunkVersion);
            stream.WriteByte(1); //looks like only layer 0 is stored in SubChunk idk
            stream.WriteByte(unchecked((byte)(sbyte)absoluteY));

            var runtimeIds = new int[subChunk.Palette.Count];
            for (int i = 0; i < runtimeIds.Length; i++)
            {
                runtimeIds[i] = StateConverter.GetHash(subChunk.Palette[i], protocol);
            }
            WritePalettedStorage(stream, subChunk.Blocks, runtimeIds);

            foreach (var blockEntity in GetBlockEntitiesInSubChunk(absoluteY))
            {
                stream.Write(SerializeNetworkNbt(blockEntity));
            }

            return stream.ToArray();
        }

        private readonly List<NbtCompound> BlockEntities = new List<NbtCompound>();

        private static readonly string[] ServerOnlyTags = ["Items"];

        public static bool TryGetPosition(NbtCompound blockEntity, out int x, out int y, out int z)
        {
            x = y = z = 0;
            if (blockEntity.Get<NbtInt>("x") is not NbtInt tx || blockEntity.Get<NbtInt>("y") is not NbtInt ty || blockEntity.Get<NbtInt>("z") is not NbtInt tz)
            {
                return false;
            }
            x = tx.Value;
            y = ty.Value;
            z = tz.Value;
            return true;
        }

        public List<NbtCompound> GetBlockEntities()
        {
            lock (BlockEntities)
            {
                return new List<NbtCompound>(BlockEntities);
            }
        }

        public NbtCompound? GetBlockEntity(int x, int y, int z)
        {
            lock (BlockEntities)
            {
                return BlockEntities.Find(be => TryGetPosition(be, out int bx, out int by, out int bz) && bx == x && by == y && bz == z);
            }
        }

        public bool SetBlockEntity(NbtCompound blockEntity)
        {
            if (!TryGetPosition(blockEntity, out int x, out int y, out int z))
            {
                return false;
            }
            lock (BlockEntities)
            {
                RemoveBlockEntity(x, y, z);
                BlockEntities.Add(blockEntity);
            }
            return true;
        }

        public bool RemoveBlockEntity(int x, int y, int z)
        {
            lock (BlockEntities)
            {
                return BlockEntities.RemoveAll(be => TryGetPosition(be, out int bx, out int by, out int bz) && bx == x && by == y && bz == z) > 0;
            }
        }

        public List<NbtCompound> GetBlockEntitiesInSubChunk(int absoluteSubChunkY)
        {
            lock (BlockEntities)
            {
                return BlockEntities.FindAll(be => TryGetPosition(be, out _, out int y, out _) && (y >> 4) == absoluteSubChunkY);
            }
        }

        public void LoadBlockEntities(byte[] data)
        {
            int pos = 0;
            while (pos < data.Length)
            {
                var nbt = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = false
                };
                int read = (int)nbt.LoadFromStream(new MemoryStream(data, pos, data.Length - pos, false), NbtCompression.None);
                if (read <= 0) break;
                pos += read;

                if (nbt.RootTag is NbtCompound compound)
                {
                    SetBlockEntity(compound);
                }
            }
        }

        public static byte[] SerializeNetworkNbt(NbtCompound blockEntity)
        {
            var copy = (NbtCompound)blockEntity.Clone();
            copy.Name ??= "";
            foreach (var tag in ServerOnlyTags)
            {
                copy.Remove(tag);
            }

            var file = new NbtFile(copy)
            {
                BigEndian = false,
                UseVarInt = true
            };
            return file.SaveToBuffer(NbtCompression.None);
        }
    }
}
