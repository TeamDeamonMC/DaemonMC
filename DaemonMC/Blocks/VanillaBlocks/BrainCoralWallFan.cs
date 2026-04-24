namespace DaemonMC.Blocks
{
    public class BrainCoralWallFan : Block
    {
        public BrainCoralWallFan()
        {
            Name = "minecraft:brain_coral_wall_fan";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0.19999998807907104;

            States["coral_direction"] = 0;
        }
    }
}
