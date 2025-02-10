namespace DaemonMC.Blocks
{
    public class RedNetherBrickStairs : Block
    {
        public RedNetherBrickStairs()
        {
            Name = "minecraft:red_nether_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
