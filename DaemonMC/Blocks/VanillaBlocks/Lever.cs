namespace DaemonMC.Blocks
{
    public class Lever : Block
    {
        public Lever()
        {
            Name = "minecraft:lever";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 0;

            States["lever_direction"] = "down_east_west";
            States["open_bit"] = (byte)0;
        }
    }
}
