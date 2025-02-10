namespace DaemonMC.Blocks
{
    public class ActivatorRail : Block
    {
        public ActivatorRail()
        {
            Name = "minecraft:activator_rail";


            States["rail_data_bit"] = (byte)0;
            States["rail_direction"] = 0;
        }
    }
}
