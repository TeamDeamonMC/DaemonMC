namespace DaemonMC.Blocks
{
    public class CalibratedSculkSensor : Block
    {
        public CalibratedSculkSensor()
        {
            Name = "minecraft:calibrated_sculk_sensor";

            States["minecraft:cardinal_direction"] = "south";
            States["sculk_sensor_phase"] = 0;
        }
    }
}
