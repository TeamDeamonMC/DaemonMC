namespace DaemonMC.Blocks
{
    public class AzaleaLeaves : Block
    {
        public AzaleaLeaves()
        {
            Name = "minecraft:azalea_leaves";

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
