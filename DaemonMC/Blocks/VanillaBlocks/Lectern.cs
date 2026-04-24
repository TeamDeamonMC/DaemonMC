namespace DaemonMC.Blocks
{
    public class Lectern : Block
    {
        public Lectern()
        {
            Name = "minecraft:lectern";

            BlastResistance = 2.5;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 2.5;
            Opacity = 0.19999998807907104;

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
        }
    }
}
