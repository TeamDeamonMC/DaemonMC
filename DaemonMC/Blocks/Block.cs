using DaemonMC.Utils;
using fNbt;

namespace DaemonMC.Blocks
{
    public abstract class Block
    {
        public string Name { get; protected set; } = "minecraft:air";
        public Dictionary<string, object> States { get; set; } = new Dictionary<string, object>();

        public int GetHash()
        {
            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = GetState(),
            };

            byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);
            return Fnv1aHash.Hash32(saveToBuffer);
        }

        public NbtCompound GetState()
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

            return compound;
        }

        public Block Clone()
        {
            var cloned = (Block)MemberwiseClone();
            cloned.States = new Dictionary<string, object>(States);
            return cloned;
        }
    }
}
