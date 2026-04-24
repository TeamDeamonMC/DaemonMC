namespace DaemonMC.Blocks
{
    public class FireCoralFan : Block
    {
        public FireCoralFan()
        {
            Name = "minecraft:fire_coral_fan";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0.19999998807907104;

            States["coral_fan_direction"] = 0;
        }
    }
}
