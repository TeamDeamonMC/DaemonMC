namespace DaemonMC.Blocks
{
    public class IronTrapdoor : Block
    {
        public IronTrapdoor()
        {
            Name = "minecraft:iron_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
