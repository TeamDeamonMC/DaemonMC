namespace DaemonMC.Blocks
{
    public class Trapdoor : Block
    {
        public Trapdoor()
        {
            Name = "minecraft:trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
