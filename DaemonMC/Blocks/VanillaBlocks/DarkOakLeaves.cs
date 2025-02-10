namespace DaemonMC.Blocks
{
    public class DarkOakLeaves : Block
    {
        public DarkOakLeaves()
        {
            Name = "minecraft:dark_oak_leaves";

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
