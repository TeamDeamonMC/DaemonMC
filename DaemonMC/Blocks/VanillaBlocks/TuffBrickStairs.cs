namespace DaemonMC.Blocks
{
    public class TuffBrickStairs : Block
    {
        public TuffBrickStairs()
        {
            Name = "minecraft:tuff_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
