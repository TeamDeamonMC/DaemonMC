namespace DaemonMC.Blocks
{
    public class DaylightDetector : Block
    {
        public DaylightDetector()
        {
            Name = "minecraft:daylight_detector";


            States["redstone_signal"] = 0;
        }
    }
}
