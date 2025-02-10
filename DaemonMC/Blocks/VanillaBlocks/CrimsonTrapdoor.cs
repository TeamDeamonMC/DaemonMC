namespace DaemonMC.Blocks
{
    public class CrimsonTrapdoor : Block
    {
        public CrimsonTrapdoor()
        {
            Name = "minecraft:crimson_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
