namespace DaemonMC.Blocks
{
    public class CherryStairs : Block
    {
        public CherryStairs()
        {
            Name = "minecraft:cherry_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
