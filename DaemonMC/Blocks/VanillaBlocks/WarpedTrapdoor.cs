namespace DaemonMC.Blocks
{
    public class WarpedTrapdoor : Block
    {
        public WarpedTrapdoor()
        {
            Name = "minecraft:warped_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
