namespace DaemonMC.Blocks
{
    public class MossyStoneBrickStairs : Block
    {
        public MossyStoneBrickStairs()
        {
            Name = "minecraft:mossy_stone_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
