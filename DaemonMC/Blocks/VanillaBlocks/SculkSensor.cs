namespace DaemonMC.Blocks
{
    public class SculkSensor : Block
    {
        public SculkSensor()
        {
            Name = "minecraft:sculk_sensor";

            BlastResistance = 1.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 0.19999998807907104;

            States["sculk_sensor_phase"] = 0;
        }
    }
}
