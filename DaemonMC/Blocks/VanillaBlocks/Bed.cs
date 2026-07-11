namespace DaemonMC.Blocks
{
    public class Bed : Block
    {
        public Bed()
        {
            Name = "minecraft:bed";

            BlastResistance = 0.20000000298023224;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0.8999999985098839;

            States["direction"] = 0;
            States["head_piece_bit"] = (byte)0;
            States["occupied_bit"] = (byte)0;
        }
    }
}
