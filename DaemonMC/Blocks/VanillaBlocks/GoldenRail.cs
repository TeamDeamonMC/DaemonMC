namespace DaemonMC.Blocks
{
    public class GoldenRail : Block
    {
        public GoldenRail()
        {
            Name = "minecraft:golden_rail";

            States["rail_data_bit"] = (byte)0;
            States["rail_direction"] = 0;
        }
    }
}
