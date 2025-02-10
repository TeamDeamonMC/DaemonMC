namespace DaemonMC.Blocks
{
    public class NetherBrickStairs : Block
    {
        public NetherBrickStairs()
        {
            Name = "minecraft:nether_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
