namespace DaemonMC.Blocks
{
    public class FrostedIce : Block
    {
        public FrostedIce()
        {
            Name = "minecraft:frosted_ice";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.9800000190734863;
            Hardness = 0.5;
            Opacity = 1;

            States["age"] = 0;
        }
    }
}
