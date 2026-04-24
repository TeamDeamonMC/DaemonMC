namespace DaemonMC.Blocks
{
    public class WeatheredCopperLantern : Block
    {
        public WeatheredCopperLantern()
        {
            Name = "minecraft:weathered_copper_lantern";

            BlastResistance = 3.5;
            Brightness = 15;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3.5;
            Opacity = 0.19999998807907104;

            States["hanging"] = (byte)0;
        }
    }
}
