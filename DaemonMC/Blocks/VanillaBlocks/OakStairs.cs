namespace DaemonMC.Blocks
{
    public class OakStairs : Block
    {
        public OakStairs()
        {
            Name = "minecraft:oak_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
