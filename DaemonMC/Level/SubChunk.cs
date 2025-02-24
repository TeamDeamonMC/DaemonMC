using fNbt;

namespace DaemonMC.Level
{
    public class SubChunk
    {
        public int Version = 8;
        public int StorageSize = 0;
        public bool IsRuntime = true;
        public int BitsPerBlock = 0;
        public List<NbtCompound> Palette = new List<NbtCompound>();
        public byte[] Blocks = new byte[40960]; //really bad design
        public uint[] Words = new uint[2000]; //todo bad design
    }
}
