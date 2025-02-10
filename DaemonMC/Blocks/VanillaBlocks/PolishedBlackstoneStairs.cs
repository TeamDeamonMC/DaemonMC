namespace DaemonMC.Blocks
{
    public class PolishedBlackstoneStairs : Block
    {
        public PolishedBlackstoneStairs()
        {
            Name = "minecraft:polished_blackstone_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
