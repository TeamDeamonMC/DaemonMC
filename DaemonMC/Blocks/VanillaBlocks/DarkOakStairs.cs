namespace DaemonMC.Blocks
{
    public class DarkOakStairs : Block
    {
        public DarkOakStairs()
        {
            Name = "minecraft:dark_oak_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
