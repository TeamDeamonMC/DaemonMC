namespace DaemonMC.Blocks
{
    public class DaylightDetectorInverted : Block
    {
        public DaylightDetectorInverted()
        {
            Name = "minecraft:daylight_detector_inverted";

            States["redstone_signal"] = 0;
        }
    }
}
