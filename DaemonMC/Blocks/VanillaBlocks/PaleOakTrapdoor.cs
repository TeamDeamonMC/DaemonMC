namespace DaemonMC.Blocks
{
    public class PaleOakTrapdoor : Block
    {
        public PaleOakTrapdoor()
        {
            Name = "minecraft:pale_oak_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
