namespace DaemonMC.Blocks
{
    public class CopperTrapdoor : Block
    {
        public CopperTrapdoor()
        {
            Name = "minecraft:copper_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
