namespace DaemonMC.Blocks
{
    public class WeatheredCutCopperStairs : Block
    {
        public WeatheredCutCopperStairs()
        {
            Name = "minecraft:weathered_cut_copper_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
