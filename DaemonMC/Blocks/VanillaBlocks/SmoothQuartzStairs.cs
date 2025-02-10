namespace DaemonMC.Blocks
{
    public class SmoothQuartzStairs : Block
    {
        public SmoothQuartzStairs()
        {
            Name = "minecraft:smooth_quartz_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
