namespace DaemonMC.Blocks
{
    public class Barrel : Block
    {
        public Barrel()
        {
            Name = "minecraft:barrel";

            BlastResistance = 2.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2.5;
            Opacity = 1;

            States["facing_direction"] = 0;
            States["open_bit"] = (byte)0;
        }
    }
}
