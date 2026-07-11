namespace DaemonMC.Blocks
{
    public class Portal : Block
    {
        public Portal()
        {
            Name = "minecraft:portal";

            BlastResistance = 0;
            Brightness = 11;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = -1;
            Opacity = 1;

            States["portal_axis"] = "unknown";
        }
    }
}
