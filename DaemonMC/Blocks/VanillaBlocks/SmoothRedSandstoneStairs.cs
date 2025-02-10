namespace DaemonMC.Blocks
{
    public class SmoothRedSandstoneStairs : Block
    {
        public SmoothRedSandstoneStairs()
        {
            Name = "minecraft:smooth_red_sandstone_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
