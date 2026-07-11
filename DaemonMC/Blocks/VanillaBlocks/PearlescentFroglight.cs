namespace DaemonMC.Blocks
{
    public class PearlescentFroglight : Block
    {
        public PearlescentFroglight()
        {
            Name = "minecraft:pearlescent_froglight";

            BlastResistance = 0.30000001192092896;
            Brightness = 15;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.30000001192092896;
            Opacity = 0;

            States["pillar_axis"] = "y";
        }
    }
}
