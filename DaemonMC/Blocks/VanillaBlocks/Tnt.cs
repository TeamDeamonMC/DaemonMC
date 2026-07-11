namespace DaemonMC.Blocks
{
    public class Tnt : Block
    {
        public Tnt()
        {
            Name = "minecraft:tnt";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 1;

            States["explode_bit"] = (byte)0;
        }
    }
}
