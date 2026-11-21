using DaemonMC.Level.Format;
using DaemonMC.Network;
using DaemonMC.Utils;
using fNbt;

namespace DaemonMC.Level
{
    public class SubChunk
    {
        private const int NetworkSubChunkVersion = 9;

        public int Version { get; set; } = 8;
        public int StorageSize { get; set; } = 0;
        public bool IsRuntime { get; set; } = true;
        public int BitsPerBlock { get; set; } = 0;
        public List<NbtCompound> Palette { get; set; } = new List<NbtCompound>(); //ordered states

        private byte[]? blocks;
        public byte[] Blocks
        {
            get => blocks ??= new byte[4096];
            set => blocks = value;
        }

        public bool IsEmpty => StorageSize == 0 || Palette.Count == 0;

        public static int Index(int localX, int localY, int localZ) => localY | (localZ << 4) | (localX << 8);

        internal bool[] GetAirMask()
        {
            var mask = new bool[Palette.Count];
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] = Palette[i]["name"]?.StringValue == "minecraft:air";
            }
            return mask;
        }

        public bool IsAir()
        {
            if (IsEmpty) return true;

            var airMask = GetAirMask();
            if (Array.TrueForAll(airMask, a => a)) return true;

            var data = Blocks;
            for (int i = 0; i < 4096; i++)
            {
                int state = data[i];
                if (state < airMask.Length && !airMask[state]) return false;
            }
            return true;
        }

        public NbtCompound? GetBlockState(int localX, int localY, int localZ)
        {
            if (IsEmpty) return null;
            int state = Blocks[Index(localX, localY, localZ)];
            return state < Palette.Count ? Palette[state] : null;
        }

        public void SetBlockState(int localX, int localY, int localZ, NbtCompound state)
        {
            if (IsEmpty)
            {
                StorageSize = 1;
                BitsPerBlock = 1;
                Palette = [new global::DaemonMC.Blocks.Air().GetState()];
                blocks = new byte[4096];
            }

            int paletteIndex = NbtUtils.IndexOf(Palette, state);
            if (paletteIndex == -1)
            {
                Palette.Add(state);
                paletteIndex = Palette.Count - 1;
                BitsPerBlock = Math.Max(BitsPerBlock, ChunkUtils.GetBitsPerBlock(Palette.Count));
            }

            Blocks[Index(localX, localY, localZ)] = (byte)paletteIndex;
        }

        public byte[] NetworkSerialize(int absoluteY, int? protocol = null, IEnumerable<NbtCompound>? blockEntities = null)
        {
            int version = protocol ?? Info.ProtocolVersion.Last();

            using var stream = new MemoryStream();
            stream.WriteByte(NetworkSubChunkVersion);
            stream.WriteByte(1); //looks like only layer 0 is stored in SubChunk idk
            stream.WriteByte(unchecked((byte)(sbyte)absoluteY));

            var runtimeIds = new int[Palette.Count];
            for (int i = 0; i < runtimeIds.Length; i++)
            {
                runtimeIds[i] = StateConverter.GetHash(Palette[i], version);
            }
            ChunkUtils.WritePalettedStorage(stream, Blocks, runtimeIds);

            if (blockEntities != null)
            {
                foreach (var blockEntity in blockEntities)
                {
                    stream.Write(ChunkColumn.SerializeNetworkNbt(blockEntity));
                }
            }

            return stream.ToArray();
        }
    }
}
