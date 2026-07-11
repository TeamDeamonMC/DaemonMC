namespace DaemonMC.Blocks
{
    public class Basalt : Block
    {
        public Basalt()
        {
            Name = "minecraft:basalt";

            BlastResistance = 4.199999809265137;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.25;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
