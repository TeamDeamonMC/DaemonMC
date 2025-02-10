namespace DaemonMC.Blocks
{
    public class MangroveTrapdoor : Block
    {
        public MangroveTrapdoor()
        {
            Name = "minecraft:mangrove_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
