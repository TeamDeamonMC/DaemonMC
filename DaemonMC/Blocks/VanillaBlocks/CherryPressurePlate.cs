namespace DaemonMC.Blocks
{
    public class CherryPressurePlate : Block
    {
        public CherryPressurePlate()
        {
            Name = "minecraft:cherry_pressure_plate";

            States["redstone_signal"] = 0;
        }
    }
}
