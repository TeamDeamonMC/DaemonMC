namespace DaemonMC.Blocks
{
    public class SprucePressurePlate : Block
    {
        public SprucePressurePlate()
        {
            Name = "minecraft:spruce_pressure_plate";

            States["redstone_signal"] = 0;
        }
    }
}
