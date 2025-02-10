namespace DaemonMC.Blocks
{
    public class AcaciaPressurePlate : Block
    {
        public AcaciaPressurePlate()
        {
            Name = "minecraft:acacia_pressure_plate";

            States["redstone_signal"] = 0;
        }
    }
}
