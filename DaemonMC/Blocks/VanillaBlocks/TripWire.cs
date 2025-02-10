namespace DaemonMC.Blocks
{
    public class TripWire : Block
    {
        public TripWire()
        {
            Name = "minecraft:trip_wire";


            States["attached_bit"] = (byte)0;
            States["disarmed_bit"] = (byte)0;
            States["powered_bit"] = (byte)0;
            States["suspended_bit"] = (byte)0;
        }
    }
}
