namespace DaemonMC.Blocks
{
    public class OakLog : Block
    {
        public OakLog()
        {
            Name = "minecraft:oak_log";

            BlastResistance = 2;
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
