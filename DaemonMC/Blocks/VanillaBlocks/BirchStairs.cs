namespace DaemonMC.Blocks
{
    public class BirchStairs : Block
    {
        public BirchStairs()
        {
            Name = "minecraft:birch_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
