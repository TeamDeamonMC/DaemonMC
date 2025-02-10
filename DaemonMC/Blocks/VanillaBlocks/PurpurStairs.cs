namespace DaemonMC.Blocks
{
    public class PurpurStairs : Block
    {
        public PurpurStairs()
        {
            Name = "minecraft:purpur_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
