namespace DaemonMC.Blocks
{
    public class Observer : Block
    {
        public Observer()
        {
            Name = "minecraft:observer";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 1;

            States["minecraft:facing_direction"] = "down";
            States["powered_bit"] = (byte)0;
        }
    }
}
