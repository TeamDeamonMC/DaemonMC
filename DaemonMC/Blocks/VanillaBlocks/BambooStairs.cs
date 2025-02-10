namespace DaemonMC.Blocks
{
    public class BambooStairs : Block
    {
        public BambooStairs()
        {
            Name = "minecraft:bamboo_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
