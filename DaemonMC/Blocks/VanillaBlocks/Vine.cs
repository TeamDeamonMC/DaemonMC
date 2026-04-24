namespace DaemonMC.Blocks
{
    public class Vine : Block
    {
        public Vine()
        {
            Name = "minecraft:vine";

            BlastResistance = 0.20000000298023224;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0;

            States["vine_direction_bits"] = 0;
        }
    }
}
