namespace DaemonMC.Blocks
{
    public class QuartzStairs : Block
    {
        public QuartzStairs()
        {
            Name = "minecraft:quartz_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
