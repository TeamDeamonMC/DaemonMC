namespace DaemonMC.Blocks
{
    public class OakLeaves : Block
    {
        public OakLeaves()
        {
            Name = "minecraft:oak_leaves";


            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
