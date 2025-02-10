namespace DaemonMC.Blocks
{
    public class ResinBrickStairs : Block
    {
        public ResinBrickStairs()
        {
            Name = "minecraft:resin_brick_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
