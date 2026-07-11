namespace DaemonMC.Blocks
{
    public class SpruceWood : Block
    {
        public SpruceWood()
        {
            Name = "minecraft:spruce_wood";

            BlastResistance = 2;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 5;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
