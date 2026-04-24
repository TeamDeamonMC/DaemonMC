namespace DaemonMC.Blocks
{
    public class CherryLog : Block
    {
        public CherryLog()
        {
            Name = "minecraft:cherry_log";

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
