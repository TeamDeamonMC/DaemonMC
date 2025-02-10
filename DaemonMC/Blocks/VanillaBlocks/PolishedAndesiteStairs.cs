namespace DaemonMC.Blocks
{
    public class PolishedAndesiteStairs : Block
    {
        public PolishedAndesiteStairs()
        {
            Name = "minecraft:polished_andesite_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
