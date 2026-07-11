namespace DaemonMC.Blocks
{
    public class TurtleEgg : Block
    {
        public TurtleEgg()
        {
            Name = "minecraft:turtle_egg";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 1;

            States["cracked_state"] = "no_cracks";
            States["turtle_egg_count"] = "one_egg";
        }
    }
}
