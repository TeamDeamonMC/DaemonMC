namespace DaemonMC.Blocks
{
    public class TripWire : Block
    {
        public TripWire()
        {
            Name = "minecraft:trip_wire";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["attached_bit"] = (byte)0;
            States["disarmed_bit"] = (byte)0;
            States["powered_bit"] = (byte)0;
            States["suspended_bit"] = (byte)0;
        }
    }
}
