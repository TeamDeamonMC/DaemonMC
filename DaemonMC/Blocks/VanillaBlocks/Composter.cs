namespace DaemonMC.Blocks
{
    public class Composter : Block
    {
        public Composter()
        {
            Name = "minecraft:composter";

            BlastResistance = 0.6000000238418579;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 0.6000000238418579;
            Opacity = 0.19999998807907104;

            States["composter_fill_level"] = 0;
        }
    }
}
