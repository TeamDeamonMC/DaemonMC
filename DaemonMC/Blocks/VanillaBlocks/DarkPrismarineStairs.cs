namespace DaemonMC.Blocks
{
    public class DarkPrismarineStairs : Block
    {
        public DarkPrismarineStairs()
        {
            Name = "minecraft:dark_prismarine_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
