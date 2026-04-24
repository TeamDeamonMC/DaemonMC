namespace DaemonMC.Blocks
{
    public class StickyPiston : Block
    {
        public StickyPiston()
        {
            Name = "minecraft:sticky_piston";

            BlastResistance = 1.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 0.19999998807907104;

            States["facing_direction"] = 0;
        }
    }
}
