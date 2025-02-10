namespace DaemonMC.Blocks
{
    public class PolishedDeepslateStairs : Block
    {
        public PolishedDeepslateStairs()
        {
            Name = "minecraft:polished_deepslate_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
