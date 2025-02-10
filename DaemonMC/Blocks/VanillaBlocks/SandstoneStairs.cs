namespace DaemonMC.Blocks
{
    public class SandstoneStairs : Block
    {
        public SandstoneStairs()
        {
            Name = "minecraft:sandstone_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
