namespace DaemonMC.Blocks
{
    public class Lava : Block
    {
        public Lava()
        {
            Name = "minecraft:lava";

            BlastResistance = 100;
            Brightness = 15;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 100;
            Opacity = 0;

            States["liquid_depth"] = 0;
        }
    }
}
