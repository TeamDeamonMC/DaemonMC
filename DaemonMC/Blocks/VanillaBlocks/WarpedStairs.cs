namespace DaemonMC.Blocks
{
    public class WarpedStairs : Block
    {
        public WarpedStairs()
        {
            Name = "minecraft:warped_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
