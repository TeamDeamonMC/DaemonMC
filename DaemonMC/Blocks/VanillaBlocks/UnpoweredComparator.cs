namespace DaemonMC.Blocks
{
    public class UnpoweredComparator : Block
    {
        public UnpoweredComparator()
        {
            Name = "minecraft:unpowered_comparator";


            States["minecraft:cardinal_direction"] = "south";
            States["output_lit_bit"] = (byte)0;
            States["output_subtract_bit"] = (byte)0;
        }
    }
}
