namespace DaemonMC.Blocks
{
    public class PolishedTuffStairs : Block
    {
        public PolishedTuffStairs()
        {
            Name = "minecraft:polished_tuff_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
