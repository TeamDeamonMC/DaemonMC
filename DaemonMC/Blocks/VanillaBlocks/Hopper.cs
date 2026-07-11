namespace DaemonMC.Blocks
{
    public class Hopper : Block
    {
        public Hopper()
        {
            Name = "minecraft:hopper";

            BlastResistance = 4.800000190734863;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 0.19999998807907104;

            States["facing_direction"] = 0;
            States["toggle_bit"] = (byte)0;
        }
    }
}
