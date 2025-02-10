namespace DaemonMC.Blocks
{
    public class EndBrickStairs : Block
    {
        public EndBrickStairs()
        {
            Name = "minecraft:end_brick_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
