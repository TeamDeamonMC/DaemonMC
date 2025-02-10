namespace DaemonMC.Blocks
{
    public class BirchTrapdoor : Block
    {
        public BirchTrapdoor()
        {
            Name = "minecraft:birch_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
