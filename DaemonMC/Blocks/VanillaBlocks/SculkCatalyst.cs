namespace DaemonMC.Blocks
{
    public class SculkCatalyst : Block
    {
        public SculkCatalyst()
        {
            Name = "minecraft:sculk_catalyst";

            BlastResistance = 3;
            Brightness = 6;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 1;

            States["bloom"] = (byte)0;
        }
    }
}
