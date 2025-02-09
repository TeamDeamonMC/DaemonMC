using DaemonMC.Utils;
using fNbt;

namespace DaemonMC.Blocks
{
    public abstract class Block
    {
        protected string Name = "minecraft:air";

        public int GetHash()
        {
            NbtCompound compound = new NbtCompound("") {
                            new NbtString("name", Name),
                            new NbtCompound("states"),
                        };

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = compound,
            };

            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);

            int hash = Fnv1aHash.Hash32(saveToBuffer);

            return hash;
        }
    }
}
