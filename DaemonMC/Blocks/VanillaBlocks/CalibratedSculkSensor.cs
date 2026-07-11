namespace DaemonMC.Blocks
{
    public class CalibratedSculkSensor : Block
    {
        public CalibratedSculkSensor()
        {
            Name = "minecraft:calibrated_sculk_sensor";

            BlastResistance = 1.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 0.19999998807907104;

            States["minecraft:cardinal_direction"] = "south";
            States["sculk_sensor_phase"] = 0;
        }
    }
}
