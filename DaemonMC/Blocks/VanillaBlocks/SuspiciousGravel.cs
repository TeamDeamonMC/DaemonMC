namespace DaemonMC.Blocks
{
    public class SuspiciousGravel : Block
    {
        public SuspiciousGravel()
        {
            Name = "minecraft:suspicious_gravel";

            BlastResistance = 0.25;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.25;
            Opacity = 1;

            States["brushed_progress"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
