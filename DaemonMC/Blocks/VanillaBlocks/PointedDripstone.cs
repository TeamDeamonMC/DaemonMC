namespace DaemonMC.Blocks
{
    public class PointedDripstone : Block
    {
        public PointedDripstone()
        {
            Name = "minecraft:pointed_dripstone";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["dripstone_thickness"] = "tip";
            States["hanging"] = (byte)0;
        }
    }
}
