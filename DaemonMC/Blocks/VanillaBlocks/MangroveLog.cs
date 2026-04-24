namespace DaemonMC.Blocks
{
    public class MangroveLog : Block
    {
        public MangroveLog()
        {
            Name = "minecraft:mangrove_log";

            BlastResistance = 0.4000000059604645;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 5;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
