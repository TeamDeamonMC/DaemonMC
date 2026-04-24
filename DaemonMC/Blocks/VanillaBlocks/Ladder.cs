namespace DaemonMC.Blocks
{
    public class Ladder : Block
    {
        public Ladder()
        {
            Name = "minecraft:ladder";

            BlastResistance = 0.4000000059604645;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.4000000059604645;
            Opacity = 0;

            States["facing_direction"] = 0;
        }
    }
}
