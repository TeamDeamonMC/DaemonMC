using DaemonMC.Network;
using fNbt;

namespace DaemonMC.Level.Format
{
    public class StateConverter //Todo rewrite all this class
    {
        public static List<NbtCompound> process(List<NbtCompound> palette, int protocol)
        {
            if (protocol <= Info.v1_21_100)
            {
                palette = To1_21_100(palette);
            }
            if (protocol <= Info.v1_21_50)
            {
                palette = To1_21_50(palette);
            }

            return palette;
        }

        static List<NbtCompound> To1_21_100(List<NbtCompound> palette)
        {
            foreach (var block in palette)
            {
                if (block.TryGet<NbtString>("name", out var nameTag))
                {
                    string name = nameTag.StringValue;

                    if (name == "minecraft:iron_chain")
                    {
                        block["name"] = new NbtString("name", "minecraft:chain");
                    }

                    if (name == "minecraft:lightning_rod")
                    {
                        if (block.TryGet<NbtCompound>("states", out var statesTag))
                        {
                            NbtCompound newStates = new NbtCompound();

                            foreach (var tag in statesTag)
                            {
                                if (tag.Name != "powered_bit")
                                {
                                    newStates.Add((NbtTag)tag.Clone());
                                }
                            }

                            newStates.Name = "states";
                            block["states"] = newStates;
                        }
                    }
                }
            }
            return palette;
        }

        static List<NbtCompound> To1_21_50(List<NbtCompound> palette)
        {
            foreach (var block in palette)
            {
                if (block.TryGet<NbtString>("name", out var nameTag))
                {
                    string name = nameTag.StringValue;
                    if (name.EndsWith("_door") || name.EndsWith("_fence_gate"))
                    {
                        if (block.TryGet<NbtCompound>("states", out var statesTag))
                        {
                            if (statesTag.TryGet<NbtString>("minecraft:cardinal_direction", out var cardinalTag))
                            {
                                int direction = GetDirection(cardinalTag.StringValue);

                                NbtCompound newStates = new NbtCompound();
                                newStates.Add(new NbtInt("direction", direction));
                                foreach (var tag in statesTag)
                                {
                                    if (tag.Name != "minecraft:cardinal_direction")
                                    {
                                        newStates.Add((NbtTag)tag.Clone());
                                    }
                                }

                                newStates.Name = "states";
                                block["states"] = newStates;
                            }
                        }
                    }
                }
            }

            return palette;
        }

        static int GetDirection(string cardinal)
        {
            return cardinal.ToLower() switch
            {
                "south" => 0,
                "west" => 1,
                "north" => 2,
                "east" => 3,
                _ => -1
            };
        }
    }
}
