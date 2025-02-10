namespace DaemonMC.Blocks
{
    public class BambooTrapdoor : Block
    {
        public BambooTrapdoor()
        {
            Name = "minecraft:bamboo_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
