namespace DaemonMC.Blocks
{
    public class TuffStairs : Block
    {
        public TuffStairs()
        {
            Name = "minecraft:tuff_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
