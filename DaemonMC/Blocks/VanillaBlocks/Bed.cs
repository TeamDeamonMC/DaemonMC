namespace DaemonMC.Blocks
{
    public class Bed : Block
    {
        public Bed()
        {
            Name = "minecraft:bed";

            States["direction"] = 0;
            States["head_piece_bit"] = (byte)0;
            States["occupied_bit"] = (byte)0;
        }
    }
}
