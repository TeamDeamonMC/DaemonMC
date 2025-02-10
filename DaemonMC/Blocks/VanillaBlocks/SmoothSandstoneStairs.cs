namespace DaemonMC.Blocks
{
    public class SmoothSandstoneStairs : Block
    {
        public SmoothSandstoneStairs()
        {
            Name = "minecraft:smooth_sandstone_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
