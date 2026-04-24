namespace DaemonMC.Blocks
{
    public class Cocoa : Block
    {
        public Cocoa()
        {
            Name = "minecraft:cocoa";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0;

            States["age"] = 0;
            States["direction"] = 0;
        }
    }
}
