namespace DaemonMC.Blocks
{
    public class AcaciaLeaves : Block
    {
        public AcaciaLeaves()
        {
            Name = "minecraft:acacia_leaves";

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
