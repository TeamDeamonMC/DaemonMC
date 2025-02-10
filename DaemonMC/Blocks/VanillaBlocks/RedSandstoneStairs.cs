namespace DaemonMC.Blocks
{
    public class RedSandstoneStairs : Block
    {
        public RedSandstoneStairs()
        {
            Name = "minecraft:red_sandstone_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
