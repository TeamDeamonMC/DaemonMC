namespace DaemonMC.Blocks
{
    public class CutCopperStairs : Block
    {
        public CutCopperStairs()
        {
            Name = "minecraft:cut_copper_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
