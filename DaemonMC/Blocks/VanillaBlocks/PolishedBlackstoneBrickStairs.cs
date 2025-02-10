namespace DaemonMC.Blocks
{
    public class PolishedBlackstoneBrickStairs : Block
    {
        public PolishedBlackstoneBrickStairs()
        {
            Name = "minecraft:polished_blackstone_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
