namespace DaemonMC.Blocks
{
    public class SpruceLeaves : Block
    {
        public SpruceLeaves()
        {
            Name = "minecraft:spruce_leaves";

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
