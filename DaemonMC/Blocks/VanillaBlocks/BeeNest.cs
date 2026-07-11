namespace DaemonMC.Blocks
{
    public class BeeNest : Block
    {
        public BeeNest()
        {
            Name = "minecraft:bee_nest";

            BlastResistance = 0.30000001192092896;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 0.30000001192092896;
            Opacity = 1;

            States["direction"] = 0;
            States["honey_level"] = 0;
        }
    }
}
