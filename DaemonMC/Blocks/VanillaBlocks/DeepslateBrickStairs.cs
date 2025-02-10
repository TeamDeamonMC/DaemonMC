namespace DaemonMC.Blocks
{
    public class DeepslateBrickStairs : Block
    {
        public DeepslateBrickStairs()
        {
            Name = "minecraft:deepslate_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
