namespace DaemonMC.Blocks
{
    public class StrippedCrimsonHyphae : Block
    {
        public StrippedCrimsonHyphae()
        {
            Name = "minecraft:stripped_crimson_hyphae";

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
