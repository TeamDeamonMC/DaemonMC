namespace DaemonMC.Blocks
{
    public class PoweredComparator : Block
    {
        public PoweredComparator()
        {
            Name = "minecraft:powered_comparator";

            States["minecraft:cardinal_direction"] = "south";
            States["output_lit_bit"] = (byte)0;
            States["output_subtract_bit"] = (byte)0;
        }
    }
}
