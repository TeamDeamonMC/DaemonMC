using DaemonMC.Utils;
using fNbt;

namespace DaemonMC.Blocks
{
    public abstract class Block
    {
        protected string Name = "minecraft:air";
        protected Dictionary<string, object> States = new Dictionary<string, object>();

        public int GetHash()
        {
            var statesCompound = new NbtCompound("states");

            foreach (var state in States)
            {
                if (state.Value is int intValue)
                {
                    statesCompound.Add(new NbtInt(state.Key, intValue));
                }
                else if (state.Value is byte byteValue)
                {
                    statesCompound.Add(new NbtByte(state.Key, byteValue));
                }
                else if (state.Value is string stringValue)
                {
                    statesCompound.Add(new NbtString(state.Key, stringValue));
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported state type for {state.Key}");
                }
            }

            NbtCompound compound = new NbtCompound("")
            {
                new NbtString("name", Name),
                statesCompound
            };

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = compound,
            };

            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);
            return Fnv1aHash.Hash32(saveToBuffer);
        }
    }
}
