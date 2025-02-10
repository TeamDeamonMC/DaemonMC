namespace DaemonMC.Blocks
{
    public class CherryTrapdoor : Block
    {
        public CherryTrapdoor()
        {
            Name = "minecraft:cherry_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
