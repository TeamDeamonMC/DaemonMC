namespace DaemonMC.Blocks
{
    public class Dispenser : Block
    {
        public Dispenser()
        {
            Name = "minecraft:dispenser";

            BlastResistance = 3.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3.5;
            Opacity = 1;

            States["facing_direction"] = 0;
            States["triggered_bit"] = (byte)0;
        }
    }
}
