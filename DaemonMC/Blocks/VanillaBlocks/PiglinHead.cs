namespace DaemonMC.Blocks
{
    public class PiglinHead : Block
    {
        public PiglinHead()
        {
            Name = "minecraft:piglin_head";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0;

            States["facing_direction"] = 0;
        }
    }
}
