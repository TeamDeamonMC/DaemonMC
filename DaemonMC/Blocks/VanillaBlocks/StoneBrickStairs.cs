namespace DaemonMC.Blocks
{
    public class StoneBrickStairs : Block
    {
        public StoneBrickStairs()
        {
            Name = "minecraft:stone_brick_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
