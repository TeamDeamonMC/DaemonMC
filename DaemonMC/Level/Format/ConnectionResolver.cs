using DaemonMC.Blocks;
using fNbt;

namespace DaemonMC.Level.Format
{
    internal static class ConnectionResolver
    {
        public delegate NbtCompound? NeighborLookup(int dx, int dy, int dz);

        private const string East = "minecraft:connection_east";
        private const string North = "minecraft:connection_north";
        private const string South = "minecraft:connection_south";
        private const string West = "minecraft:connection_west";
        private const string Up = "minecraft:connection_up";
        private const string Down = "minecraft:connection_down";
        private const string Corner = "minecraft:corner";

        public static readonly HashSet<string> ComputedStates = new(StringComparer.Ordinal)
        {
            East, North, South, West, Up, Down, Corner,
            "redstone_east", "redstone_north", "redstone_south", "redstone_west"
        };

        private enum Dir { North, East, South, West, Up, Down }

        private static readonly Dir[] Horizontal = [Dir.North, Dir.East, Dir.South, Dir.West];

        private static (int dx, int dy, int dz) Offset(Dir dir) => dir switch
        {
            Dir.North => (0, 0, -1),
            Dir.South => (0, 0, 1),
            Dir.East => (1, 0, 0),
            Dir.West => (-1, 0, 0),
            Dir.Up => (0, 1, 0),
            _ => (0, -1, 0)
        };

        private static Dir Opposite(Dir dir) => dir switch
        {
            Dir.North => Dir.South,
            Dir.South => Dir.North,
            Dir.East => Dir.West,
            Dir.West => Dir.East,
            Dir.Up => Dir.Down,
            _ => Dir.Up
        };

        private static Dir CounterClockwise(Dir dir) => dir switch
        {
            Dir.North => Dir.West,
            Dir.West => Dir.South,
            Dir.South => Dir.East,
            _ => Dir.North
        };

        private static bool SameAxis(Dir a, Dir b) => a == b || a == Opposite(b);

        private static string ConnectionState(Dir dir) => dir switch
        {
            Dir.North => North,
            Dir.South => South,
            Dir.East => East,
            Dir.West => West,
            Dir.Up => Up,
            _ => Down
        };

        public static bool Resolve(StateConverter.BlockState state, NeighborLookup lookup)
        {
            NbtCompound? At(Dir dir)
            {
                var (dx, dy, dz) = Offset(dir);
                return lookup(dx, dy, dz);
            }

            string name = state.Name;
            var values = new Dictionary<string, object>();

            if (IsPaneOrBars(name))
            {
                foreach (var dir in Horizontal)
                {
                    var n = At(dir);
                    values[ConnectionState(dir)] = Bit(IsPaneOrBars(Name(n)) || IsWall(Name(n)) || IsSolid(n));
                }
            }
            else if (name.EndsWith("_fence", StringComparison.Ordinal))
            {
                foreach (var dir in Horizontal)
                {
                    var n = At(dir);
                    values[ConnectionState(dir)] = Bit(FenceConnects(name, n, dir) || IsSolid(n));
                }
            }
            else if (name == "minecraft:trip_wire")
            {
                foreach (var dir in Horizontal)
                {
                    var n = At(dir);
                    values[ConnectionState(dir)] = Bit(Name(n) == "minecraft:trip_wire" ||
                        (Name(n) == "minecraft:tripwire_hook" && LegacyDirection(n!, "direction") == Opposite(dir)));
                }
            }
            else if (name == "minecraft:chorus_plant")
            {
                foreach (var dir in new[] { Dir.North, Dir.East, Dir.South, Dir.West, Dir.Up, Dir.Down })
                {
                    string n = Name(At(dir));
                    values[ConnectionState(dir)] = Bit(n == "minecraft:chorus_plant" || n == "minecraft:chorus_flower" ||
                        (dir == Dir.Down && n == "minecraft:end_stone"));
                }
            }
            else if (name == "minecraft:fire")
            {
                var below = At(Dir.Down);
                bool supported = IsSolid(below) || CanBurn(below);
                foreach (var dir in new[] { Dir.North, Dir.East, Dir.South, Dir.West, Dir.Up })
                {
                    values[ConnectionState(dir)] = Bit(!supported && CanBurn(At(dir)));
                }
            }
            else if (name == "minecraft:redstone_wire")
            {
                ResolveRedstone(lookup, values);
            }
            else if (state.States.ContainsKey(Corner) && name.EndsWith("_stairs", StringComparison.Ordinal))
            {
                values[Corner] = StairCorner(state, At);
            }

            bool changed = false;
            foreach (var (key, value) in values)
            {
                if (!state.States.TryGetValue(key, out var existing))
                {
                    continue;
                }

                NbtTag tag = value is byte b ? new NbtByte(key, b) : new NbtString(key, (string)value);
                if (!tag.ToString().Equals(existing.ToString(), StringComparison.Ordinal))
                {
                    state.States[key] = tag;
                    changed = true;
                }
            }
            return changed;
        }

        private static void ResolveRedstone(NeighborLookup lookup, Dictionary<string, object> values)
        {
            bool aboveSolid = IsSolid(lookup(0, 1, 0));
            var sides = new Dictionary<Dir, string>();

            foreach (var dir in Horizontal)
            {
                var (dx, _, dz) = Offset(dir);
                var n = lookup(dx, 0, dz);

                if (RedstoneConnects(n, dir))
                {
                    sides[dir] = "side";
                }
                else if (!aboveSolid && IsSolid(n) && Name(lookup(dx, 1, dz)) == "minecraft:redstone_wire")
                {
                    sides[dir] = "up";
                }
                else if (!IsSolid(n) && Name(lookup(dx, -1, dz)) == "minecraft:redstone_wire")
                {
                    sides[dir] = "side";
                }
                else
                {
                    sides[dir] = "none";
                }
            }

            var connected = Horizontal.Where(d => sides[d] != "none").ToList();
            if (connected.Count == 0)
            {
                foreach (var dir in Horizontal) sides[dir] = "side";
            }
            else if (connected.Count == 1)
            {
                sides[Opposite(connected[0])] = "side";
            }

            values["redstone_north"] = sides[Dir.North];
            values["redstone_east"] = sides[Dir.East];
            values["redstone_south"] = sides[Dir.South];
            values["redstone_west"] = sides[Dir.West];
        }

        private static bool RedstoneConnects(NbtCompound? n, Dir dir)
        {
            string name = Name(n);
            if (name == "minecraft:redstone_wire") return true;
            if (name is "minecraft:unpowered_repeater" or "minecraft:powered_repeater")
            {
                return CardinalDirection(n!) is { } facing && SameAxis(facing, dir);
            }
            if (name == "minecraft:observer")
            {
                return StringState(n!, "minecraft:facing_direction") == dir.ToString().ToLowerInvariant();
            }
            return IsSignalSource(name);
        }

        private static string StairCorner(StateConverter.BlockState state, Func<Dir, NbtCompound?> at)
        {
            if (StairFacing(state.States) is not { } facing)
            {
                return "none";
            }
            byte half = ByteState(state.States, "upside_down_bit");

            bool SameHalfStairs(NbtCompound? n, out Dir nFacing)
            {
                nFacing = default;
                if (n == null || !Name(n).EndsWith("_stairs", StringComparison.Ordinal)) return false;
                var states = States(n);
                if (StairFacing(states) is not { } f || ByteState(states, "upside_down_bit") != half) return false;
                nFacing = f;
                return true;
            }

            bool CanTakeShape(Dir dir)
            {
                return !SameHalfStairs(at(dir), out var f) || f != facing;
            }

            if (SameHalfStairs(at(facing), out var behind) && !SameAxis(behind, facing) && CanTakeShape(Opposite(behind)))
            {
                return behind == CounterClockwise(facing) ? "outer_left" : "outer_right";
            }

            if (SameHalfStairs(at(Opposite(facing)), out var front) && !SameAxis(front, facing) && CanTakeShape(front))
            {
                return front == CounterClockwise(facing) ? "inner_left" : "inner_right";
            }

            return "none";
        }

        // weirdo_direction: 0 = east, 1 = west, 2 = south, 3 = north
        private static Dir? StairFacing(IDictionary<string, NbtTag> states)
        {
            if (!states.TryGetValue("weirdo_direction", out var tag) || tag is not NbtInt i) return null;
            return i.Value switch
            {
                0 => Dir.East,
                1 => Dir.West,
                2 => Dir.South,
                3 => Dir.North,
                _ => null
            };
        }

        private static bool FenceConnects(string fence, NbtCompound? n, Dir dir)
        {
            string name = Name(n);
            if (name.EndsWith("_fence", StringComparison.Ordinal))
            {
                return (fence == "minecraft:nether_brick_fence") == (name == "minecraft:nether_brick_fence");
            }
            if (name.EndsWith("fence_gate", StringComparison.Ordinal))
            {
                return CardinalDirection(n!) is { } facing && !SameAxis(facing, dir);
            }
            return false;
        }

        private static bool IsPaneOrBars(string name)
        {
            return name.EndsWith("glass_pane", StringComparison.Ordinal) || name.EndsWith("_bars", StringComparison.Ordinal);
        }

        private static bool IsWall(string name)
        {
            return name.EndsWith("_wall", StringComparison.Ordinal) && !name.Contains("sign") && !name.Contains("banner") &&
                   !name.Contains("torch") && !name.Contains("fan") && !name.Contains("head") && !name.Contains("skull");
        }

        private static readonly string[] SignalSources =
        [
            "button", "pressure_plate", "lever", "redstone_torch", "redstone_block", "daylight_detector",
            "comparator", "trapped_chest", "tripwire_hook", "detector_rail", "sculk_sensor"
        ];

        private static bool IsSignalSource(string name)
        {
            return name is "minecraft:target" or "minecraft:lectern" || SignalSources.Any(s => name.Contains(s, StringComparison.Ordinal));
        }

        private static readonly string[] NonSolid =
        [
            "air", "water", "lava", "_slab", "stairs", "fence", "_wall", "pane", "_bars", "door", "button", "pressure_plate",
            "torch", "lantern", "sign", "banner", "carpet", "rail", "ladder", "vine", "flower", "sapling", "grass", "fern",
            "bush", "mushroom", "coral", "kelp", "seagrass", "sea_pickle", "candle", "chain", "rod", "flower_pot", "head",
            "skull", "minecraft:bed", "chest", "cake", "cactus", "snow_layer", "redstone_wire", "repeater", "comparator", "lever",
            "trip_wire", "tripwire_hook", "fire", "frame", "campfire", "anvil", "bell", "brewing_stand", "cauldron", "hopper",
            "scaffolding", "waterlily", "lily_pad", "chorus", "portal", "web", "wheat", "carrots", "potatoes", "beetroot",
            "reeds", "sugar_cane", "minecraft:bamboo", "dripstone", "amethyst_cluster", "_bud", "lichen", "sculk_vein", "azalea",
            "deadbush", "dead_bush", "roots", "fungus", "sprouts", "nether_wart", "pumpkin_stem", "melon_stem", "cocoa", "berry",
            "dripleaf", "spore_blossom", "hanging", "petals", "egg", "conduit", "enchanting_table", "daylight_detector",
            "lectern", "stonecutter", "grindstone", "composter", "light_block", "structure_void", "leaf_litter", "eyeblossom",
            "pointed", "heavy_core", "decorated_pot", "shelf"
        ];

        private static readonly HashSet<string> SolidExceptions = new(StringComparer.Ordinal)
        {
            "minecraft:bedrock", "minecraft:sea_lantern", "minecraft:mushroom_stem", "minecraft:bamboo_planks", "minecraft:bamboo_mosaic"
        };

        private static bool IsSolid(NbtCompound? n)
        {
            if (n == null) return false;
            string name = Name(n);
            if (SolidExceptions.Contains(name) || name.EndsWith("double_slab", StringComparison.Ordinal)) return true;
            if (name.EndsWith("_block", StringComparison.Ordinal) && !name.Contains("light_block") && !name.Contains("structure_void")) return true;
            return !NonSolid.Any(s => name.Contains(s, StringComparison.Ordinal));
        }

        private static bool CanBurn(NbtCompound? n)
        {
            return n != null && BlockPalette.GetDefaultBlock(Name(n)) is { } block && block.FlameEncouragement > 0;
        }

        // tripwire_hook direction: 0 = south, 1 = west, 2 = north, 3 = east
        private static Dir? LegacyDirection(NbtCompound n, string state)
        {
            return States(n).TryGetValue(state, out var tag) && tag is NbtInt i ? i.Value switch
            {
                0 => Dir.South,
                1 => Dir.West,
                2 => Dir.North,
                3 => Dir.East,
                _ => null
            } : null;
        }

        private static Dir? CardinalDirection(NbtCompound n)
        {
            return StringState(n, "minecraft:cardinal_direction") switch
            {
                "north" => Dir.North,
                "south" => Dir.South,
                "east" => Dir.East,
                "west" => Dir.West,
                _ => null
            };
        }

        private static object Bit(bool value) => value ? (byte)1 : (byte)0;

        private static string Name(NbtCompound? n) => n?.Get<NbtString>("name")?.Value ?? "minecraft:air";

        private static IDictionary<string, NbtTag> States(NbtCompound n)
        {
            var result = new Dictionary<string, NbtTag>(StringComparer.Ordinal);
            if (n.TryGet<NbtCompound>("states", out var states))
            {
                foreach (var tag in states!.Tags)
                {
                    result[tag.Name!] = tag;
                }
            }
            return result;
        }

        private static string? StringState(NbtCompound n, string state)
        {
            return States(n).TryGetValue(state, out var tag) && tag is NbtString s ? s.Value : null;
        }

        private static byte ByteState(IDictionary<string, NbtTag> states, string state)
        {
            return states.TryGetValue(state, out var tag) && tag is NbtByte b ? b.Value : (byte)0;
        }
    }
}
