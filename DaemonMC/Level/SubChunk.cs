using fNbt;

namespace DaemonMC.Level
{
    public class SubChunk
    {
        public int Version { get; set; } = 8;
        public int StorageSize { get; set; } = 0;
        public bool IsRuntime { get; set; } = true;
        public int BitsPerBlock { get; set; } = 0;
        public List<NbtCompound> Palette { get; set; } = new List<NbtCompound>(); //ordered states
        public byte[] Blocks { get; set; } = new byte[4096]; //blocks (index from ordered states)
    }
}
