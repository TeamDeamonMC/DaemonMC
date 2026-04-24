namespace DaemonMC.Blocks
{
    public class EndRod : Block
    {
        public EndRod()
        {
            Name = "minecraft:end_rod";

            BlastResistance = 0;
            Brightness = 14;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["facing_direction"] = 0;
        }
    }
}
