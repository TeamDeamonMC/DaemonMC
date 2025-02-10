namespace DaemonMC.Blocks
{
    public class PrismarineBricksStairs : Block
    {
        public PrismarineBricksStairs()
        {
            Name = "minecraft:prismarine_bricks_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
