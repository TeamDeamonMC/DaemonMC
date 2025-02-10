namespace DaemonMC.Blocks
{
    public class ExposedCutCopperStairs : Block
    {
        public ExposedCutCopperStairs()
        {
            Name = "minecraft:exposed_cut_copper_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
