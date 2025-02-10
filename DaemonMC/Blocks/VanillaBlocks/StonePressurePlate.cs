namespace DaemonMC.Blocks
{
    public class StonePressurePlate : Block
    {
        public StonePressurePlate()
        {
            Name = "minecraft:stone_pressure_plate";

            States["redstone_signal"] = 0;
        }
    }
}
