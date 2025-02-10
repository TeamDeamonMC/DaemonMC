namespace DaemonMC.Blocks
{
    public class DarkOakTrapdoor : Block
    {
        public DarkOakTrapdoor()
        {
            Name = "minecraft:dark_oak_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
