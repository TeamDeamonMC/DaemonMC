namespace DaemonMC.Blocks
{
    public class SmoothQuartz : Block
    {
        public SmoothQuartz()
        {
            Name = "minecraft:smooth_quartz";

            BlastResistance = 6;
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
