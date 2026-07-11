namespace DaemonMC.Blocks
{
    public class CrimsonStem : Block
    {
        public CrimsonStem()
        {
            Name = "minecraft:crimson_stem";

            BlastResistance = 2;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
