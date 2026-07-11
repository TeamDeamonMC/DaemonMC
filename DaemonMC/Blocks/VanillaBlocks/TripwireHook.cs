namespace DaemonMC.Blocks
{
    public class TripwireHook : Block
    {
        public TripwireHook()
        {
            Name = "minecraft:tripwire_hook";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["attached_bit"] = (byte)0;
            States["direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
