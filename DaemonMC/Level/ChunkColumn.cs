using DaemonMC.Biomes;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using fNbt;

namespace DaemonMC.Level
{
    public class ChunkColumn
    {
        public int X { get; }
        public int Z { get; }
        public Dimension Dimension { get; }
        public bool Generated { get; internal set; }
        public int HighestSubChunk { get; internal set; } = -1;
        public BiomeStorage?[] Biomes { get; }
        internal int[]? Heights;
        private readonly List<NbtCompound> BlockEntities = new List<NbtCompound>();
        private static readonly string[] ServerOnlyTags = ["Items"];
        private volatile Dictionary<(bool Override, int DefaultBiome), byte[]> biomeCache = new();
        
        public void InvalidateBiomeCache()
        {
            biomeCache = new();
        }

        public ChunkColumn(int x, int z, Dimension dimension)
        {
            X = x;
            Z = z;
            Dimension = dimension;
            Biomes = new BiomeStorage?[dimension.SubChunkCount()];
        }

        public static int[] ComputeHeights(int highestIndex, Func<int, SubChunk?> getSubChunk)
        {
            var heights = new int[256];
            Array.Fill(heights, -1);
            int remaining = 256;

            for (int i = highestIndex; i >= 0 && remaining > 0; i--)
            {
                var subChunk = getSubChunk(i);
                if (subChunk == null || subChunk.IsAir()) continue;

                var airMask = subChunk.GetAirMask();
                var blocks = subChunk.Blocks;

                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        int column = (z << 4) | x;
                        if (heights[column] != -1) continue;

                        for (int y = 15; y >= 0; y--)
                        {
                            int state = blocks[SubChunk.Index(x, y, z)];
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

        public static int HighestNonAirSubChunk(int[] heights)
        {
            int max = heights.Max();
            return max < 0 ? -1 : max >> 4;
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

        public int GetBiomeId(int localX, int subChunkIndex, int localY, int localZ)
        {
            lock (Biomes)
            {
                for (int i = Math.Min(subChunkIndex, Biomes.Length - 1); i >= 0; i--) //empty storages inherit the one below
                {
                    var storage = Biomes[i];
                    if (storage != null && storage.Palette.Count > 0)
                    {
                        return storage.Indices.Length == 0 ? storage.Palette[0] : storage.Palette[storage.Indices[SubChunk.Index(localX, localY, localZ)]];
                    }
                }
                return BiomeManager.GetDefault(Dimension).Id;
            }
        }

        public void SetBiomeId(int localX, int subChunkIndex, int localY, int localZ, int biomeId)
        {
            lock (Biomes)
            {
                MaterializeBiomes();
                var storage = Biomes[subChunkIndex]!;
                int index = storage.Palette.IndexOf(biomeId);
                if (index == 0 && storage.Indices.Length == 0) return; //already single biome
                storage.EnsureIndices();
                if (index < 0)
                {
                    if (storage.Palette.Count >= 256) storage.Compact();
                    index = storage.Palette.Count;
                    storage.Palette.Add(biomeId);
                }
                storage.Indices[SubChunk.Index(localX, localY, localZ)] = (byte)index;
                InvalidateBiomeCache();
            }
        }

        public void FillBiome(int biomeId)
        {
            lock (Biomes)
            {
                Array.Clear(Biomes);
                Biomes[0] = new BiomeStorage { Palette = [biomeId] };
                InvalidateBiomeCache();
            }
        }

        private void MaterializeBiomes()
        {
            BiomeStorage? previous = null;
            for (int i = 0; i < Biomes.Length; i++)
            {
                var storage = Biomes[i];
                if (storage == null || storage.Palette.Count == 0)
                {
                    Biomes[i] = previous?.Clone() ?? new BiomeStorage { Palette = [BiomeManager.GetDefault(Dimension).Id] };
                }
                previous = Biomes[i];
            }
        }

        public byte[] NetworkSerializeBiomes(Biome? biomeOverride = null)
        {
            int defaultBiome = (biomeOverride ?? BiomeManager.GetDefault(Dimension)).Id;
            var key = (biomeOverride != null, defaultBiome);

            var cache = biomeCache;
            lock (cache)
            {
                if (cache.TryGetValue(key, out var cached)) return cached;
            }

            byte[] result = SerializeBiomes(biomeOverride, defaultBiome);
            lock (cache)
            {
                cache[key] = result;
            }
            return result;
        }

        private byte[] SerializeBiomes(Biome? biomeOverride, int defaultBiome)
        {
            using var stream = new MemoryStream();
            byte[]? previous = null;

            lock (Biomes)
            for (int i = 0; i < Biomes.Length; i++)
            {
                byte[] current;
                var storage = biomeOverride == null ? Biomes[i] : null;
                if (storage != null && storage.Palette.Count > 0)
                {
                    using var biomeStream = new MemoryStream();
                    ChunkUtils.WritePalettedStorage(biomeStream, storage.Indices, storage.Palette);
                    current = biomeStream.ToArray();
                }
                else if (previous != null)
                {
                    current = previous;
                }
                else
                {
                    using var biomeStream = new MemoryStream();
                    ChunkUtils.WritePalettedStorage(biomeStream, [], [defaultBiome]);
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

    public class BiomeStorage
    {
        public List<int> Palette { get; set; } = new List<int>(); //ordered biomes
        public byte[] Indices { get; set; } = []; //3D biomes (index from ordered biomes), empty with single biome, performance thing

        public BiomeStorage Clone() => new BiomeStorage { Palette = new List<int>(Palette), Indices = (byte[])Indices.Clone() };

        internal void EnsureIndices()
        {
            if (Indices.Length == 0) Indices = new byte[4096]; //all 0 -> Palette[0]
        }

        internal void Compact()
        {
            if (Indices.Length == 0) return;
            var used = new bool[Palette.Count];
            foreach (var index in Indices) used[index] = true;
            var remap = new byte[Palette.Count];
            var palette = new List<int>();
            for (int i = 0; i < Palette.Count; i++)
            {
                if (!used[i]) continue;
                remap[i] = (byte)palette.Count;
                palette.Add(Palette[i]);
            }
            for (int i = 0; i < Indices.Length; i++) Indices[i] = remap[Indices[i]];
            Palette = palette;
        }
    }
}
