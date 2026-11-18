using System.Collections.Concurrent;
using DaemonMC.Network;
using DaemonMC.Utils;
using fNbt;

namespace DaemonMC.Level.Format
{
    //renamedIds              -> .RenameBlock("old", "new")
    //addedProperties         -> .AddState(block(s), "state", ...)
    //removedProperties       -> .RemoveState(block(s), "state", valueOldClientsExpect)
    //renamedProperties       -> .RenameState(block(s), "old", "new")
    //remappedPropertyValues  -> .RemapValue(block(s), "state", oldValue, newValue)
    //other                   -> .Custom(state => { ...; return changed; })

    public static class StateConverter
    {
        private static readonly Schema[] Schemas =
        [
            new Schema(Info.v1_21_111) // 1.21.100 -> 1.21.110
                .RenameBlock("minecraft:chain", "minecraft:iron_chain")
                .AddState("minecraft:lightning_rod", "powered_bit"),

            new Schema(Info.v1_26_30) // 1.26.20 -> 1.26.30
                .AddState("minecraft:potent_sulfur", "potent_sulfur_state"),

            new Schema(Info.v1_26_50) // 1.26.40 -> 1.26.50
                .AddState(EndsWith("_fence", "glass_pane", "_bars", ":trip_wire"),
                    "minecraft:connection_east", "minecraft:connection_north", "minecraft:connection_south", "minecraft:connection_west")
                .AddState(EndsWith("_stairs"), "minecraft:corner"),

            new Schema(Info.v1_26_60) // 1.26.50 -> 1.26.60
                .AddState("minecraft:chorus_plant",
                    "minecraft:connection_down", "minecraft:connection_east", "minecraft:connection_north",
                    "minecraft:connection_south", "minecraft:connection_up", "minecraft:connection_west")
                .AddState("minecraft:fire",
                    "minecraft:connection_east", "minecraft:connection_north", "minecraft:connection_south",
                    "minecraft:connection_up", "minecraft:connection_west")
                .AddState("minecraft:redstone_wire", "redstone_east", "redstone_north", "redstone_south", "redstone_west"),
        ];

        private static readonly ConcurrentDictionary<long, int> HashCache = new();

        public static int GetHash(NbtCompound latestState, int protocol)
        {
            int latestHash = Hash(latestState);
            int from = FirstSchemaToUndo(protocol);

            if (from == Schemas.Length)
            {
                return latestHash;
            }

            long key = ((long)from << 32) | (uint)latestHash;
            return HashCache.GetOrAdd(key, _ => Hash(DowngradeFrom(latestState, from)));
        }

        public static NbtCompound Downgrade(NbtCompound latestState, int protocol)
        {
            return DowngradeFrom(latestState, FirstSchemaToUndo(protocol));
        }

        public static int Hash(NbtCompound state)
        {
            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = state,
            };

            return Fnv1aHash.Hash32(nbt.SaveToBuffer(NbtCompression.None));
        }

        private static NbtCompound DowngradeFrom(NbtCompound latestState, int from)
        {
            if (from == Schemas.Length)
            {
                return latestState;
            }

            var state = new BlockState(latestState);

            for (int i = Schemas.Length - 1; i >= from; i--)
            {
                Schemas[i].Downgrade(state);
            }

            return state.Changed ? state.ToNbt() : latestState;
        }

        private static int FirstSchemaToUndo(int protocol)
        {
            for (int i = 0; i < Schemas.Length; i++)
            {
                if (Schemas[i].Protocol > protocol)
                {
                    return i;
                }
            }
            return Schemas.Length;
        }

        private static Func<string, bool> EndsWith(params string[] suffixes)
        {
            return name => suffixes.Any(s => name.EndsWith(s, StringComparison.Ordinal));
        }

        public sealed class BlockState
        {
            public string Name { get; set; }
            public SortedDictionary<string, NbtTag> States { get; } = new(StringComparer.Ordinal);
            public bool Changed { get; set; }

            public BlockState(NbtCompound nbt)
            {
                Name = nbt.TryGet<NbtString>("name", out var name) ? name!.Value : "minecraft:air";

                if (nbt.TryGet<NbtCompound>("states", out var states))
                {
                    foreach (var tag in states!.Tags)
                    {
                        States[tag.Name!] = (NbtTag)tag.Clone();
                    }
                }
            }

            public NbtCompound ToNbt()
            {
                var states = new NbtCompound("states");
                foreach (var tag in States.Values)
                {
                    states.Add(tag);
                }

                return new NbtCompound("")
                {
                    new NbtString("name", Name),
                    states
                };
            }
        }

        private sealed class Schema(int protocol)
        {
            public int Protocol { get; } = protocol;
            private readonly List<Func<BlockState, bool>> rules = new();

            public void Downgrade(BlockState state)
            {
                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    if (rules[i](state))
                    {
                        state.Changed = true;
                    }
                }
            }

            public Schema RenameBlock(string oldName, string newName)
            {
                rules.Add(s =>
                {
                    if (s.Name != newName) return false;
                    s.Name = oldName;
                    return true;
                });
                return this;
            }

            public Schema AddState(string block, params string[] states) => AddState(n => n == block, states);

            public Schema AddState(Func<string, bool> block, params string[] states)
            {
                rules.Add(s =>
                {
                    if (!block(s.Name)) return false;
                    bool changed = false;
                    foreach (var name in states)
                    {
                        changed |= s.States.Remove(name);
                    }
                    return changed;
                });
                return this;
            }

            public Schema RemoveState(string block, string state, object oldValue) => RemoveState(n => n == block, state, oldValue);

            public Schema RemoveState(Func<string, bool> block, string state, object oldValue)
            {
                rules.Add(s =>
                {
                    if (!block(s.Name) || s.States.ContainsKey(state)) return false;
                    s.States[state] = Tag(state, oldValue);
                    return true;
                });
                return this;
            }

            public Schema RenameState(string block, string oldName, string newName) => RenameState(n => n == block, oldName, newName);

            public Schema RenameState(Func<string, bool> block, string oldName, string newName)
            {
                rules.Add(s =>
                {
                    if (!block(s.Name) || !s.States.Remove(newName, out var tag)) return false;
                    s.States[oldName] = Tag(oldName, Value(tag));
                    return true;
                });
                return this;
            }

            public Schema RemapValue(string block, string state, object oldValue, object newValue) => RemapValue(n => n == block, state, oldValue, newValue);

            public Schema RemapValue(Func<string, bool> block, string state, object oldValue, object newValue)
            {
                rules.Add(s =>
                {
                    if (!block(s.Name) || !s.States.TryGetValue(state, out var tag) || !Equals(Value(tag), newValue)) return false;
                    s.States[state] = Tag(state, oldValue);
                    return true;
                });
                return this;
            }

            public Schema Custom(Func<BlockState, bool> downgrade)
            {
                rules.Add(downgrade);
                return this;
            }

            private static object Value(NbtTag tag) => tag switch
            {
                NbtByte b => b.Value,
                NbtInt i => i.Value,
                NbtString s => s.Value,
                _ => throw new InvalidOperationException($"Unsupported block state type {tag.TagType} for {tag.Name}")
            };

            private static NbtTag Tag(string name, object value) => value switch
            {
                byte b => new NbtByte(name, b),
                bool b => new NbtByte(name, (byte)(b ? 1 : 0)),
                int i => new NbtInt(name, i),
                string s => new NbtString(name, s),
                _ => throw new InvalidOperationException($"Unsupported block state value {value?.GetType().Name} for {name}")
            };
        }
    }
}
