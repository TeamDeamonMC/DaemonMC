namespace DaemonMC.Blocks
{
    public class JungleTrapdoor : Block
    {
        public JungleTrapdoor()
        {
            Name = "minecraft:jungle_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
