namespace DaemonMC.Blocks
{
    public class CobbledDeepslateStairs : Block
    {
        public CobbledDeepslateStairs()
        {
            Name = "minecraft:cobbled_deepslate_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
