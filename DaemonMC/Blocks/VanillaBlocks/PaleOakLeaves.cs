namespace DaemonMC.Blocks
{
    public class PaleOakLeaves : Block
    {
        public PaleOakLeaves()
        {
            Name = "minecraft:pale_oak_leaves";

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
