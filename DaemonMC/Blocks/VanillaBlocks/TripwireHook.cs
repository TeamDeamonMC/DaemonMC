namespace DaemonMC.Blocks
{
    public class TripwireHook : Block
    {
        public TripwireHook()
        {
            Name = "minecraft:tripwire_hook";

            States["attached_bit"] = (byte)0;
            States["direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
