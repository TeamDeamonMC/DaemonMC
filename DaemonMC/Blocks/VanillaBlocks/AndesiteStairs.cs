namespace DaemonMC.Blocks
{
    public class AndesiteStairs : Block
    {
        public AndesiteStairs()
        {
            Name = "minecraft:andesite_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
