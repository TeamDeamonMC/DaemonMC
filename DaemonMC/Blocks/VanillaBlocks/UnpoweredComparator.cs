namespace DaemonMC.Blocks
{
    public class UnpoweredComparator : Block
    {
        public UnpoweredComparator()
        {
            Name = "minecraft:unpowered_comparator";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["minecraft:cardinal_direction"] = "south";
            States["output_lit_bit"] = (byte)0;
            States["output_subtract_bit"] = (byte)0;
        }
    }
}
