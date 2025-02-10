namespace DaemonMC.Blocks
{
    public class MangrovePressurePlate : Block
    {
        public MangrovePressurePlate()
        {
            Name = "minecraft:mangrove_pressure_plate";

            States["redstone_signal"] = 0;
        }
    }
}
