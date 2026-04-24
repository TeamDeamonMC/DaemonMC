namespace DaemonMC.Blocks
{
    public class DaylightDetectorInverted : Block
    {
        public DaylightDetectorInverted()
        {
            Name = "minecraft:daylight_detector_inverted";

            BlastResistance = 0.20000000298023224;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 1;

            States["redstone_signal"] = 0;
        }
    }
}
