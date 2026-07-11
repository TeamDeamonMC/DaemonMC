namespace DaemonMC.Blocks
{
    public class Water : Block
    {
        public Water()
        {
            Name = "minecraft:water";

            BlastResistance = 100;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 100;
            Opacity = 0;

            States["liquid_depth"] = 0;
        }
    }
}
