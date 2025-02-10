namespace DaemonMC.Blocks
{
    public class WarpedPressurePlate : Block
    {
        public WarpedPressurePlate()
        {
            Name = "minecraft:warped_pressure_plate";

            States["redstone_signal"] = 0;
        }
    }
}
