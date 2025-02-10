namespace DaemonMC.Blocks
{
    public class BrickStairs : Block
    {
        public BrickStairs()
        {
            Name = "minecraft:brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
