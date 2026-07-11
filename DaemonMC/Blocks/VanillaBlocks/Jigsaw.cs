namespace DaemonMC.Blocks
{
    public class Jigsaw : Block
    {
        public Jigsaw()
        {
            Name = "minecraft:jigsaw";

            BlastResistance = 3600000;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = -1;
            Opacity = 1;

            States["facing_direction"] = 0;
            States["rotation"] = 0;
        }
    }
}
