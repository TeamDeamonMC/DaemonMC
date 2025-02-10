namespace DaemonMC.Blocks
{
    public class PrismarineStairs : Block
    {
        public PrismarineStairs()
        {
            Name = "minecraft:prismarine_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
