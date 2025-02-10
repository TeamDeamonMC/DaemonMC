namespace DaemonMC.Blocks
{
    public class PolishedGraniteStairs : Block
    {
        public PolishedGraniteStairs()
        {
            Name = "minecraft:polished_granite_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
