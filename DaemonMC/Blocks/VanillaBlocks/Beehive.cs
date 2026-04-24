namespace DaemonMC.Blocks
{
    public class Beehive : Block
    {
        public Beehive()
        {
            Name = "minecraft:beehive";

            BlastResistance = 0.6000000238418579;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 0.6000000238418579;
            Opacity = 1;

            States["direction"] = 0;
            States["honey_level"] = 0;
        }
    }
}
