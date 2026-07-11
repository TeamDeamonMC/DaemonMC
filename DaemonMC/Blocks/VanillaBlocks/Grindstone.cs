namespace DaemonMC.Blocks
{
    public class Grindstone : Block
    {
        public Grindstone()
        {
            Name = "minecraft:grindstone";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 0.19999998807907104;

            States["attachment"] = "standing";
            States["direction"] = 0;
        }
    }
}
