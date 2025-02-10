namespace DaemonMC.Blocks
{
    public class DetectorRail : Block
    {
        public DetectorRail()
        {
            Name = "minecraft:detector_rail";

            States["rail_data_bit"] = (byte)0;
            States["rail_direction"] = 0;
        }
    }
}
