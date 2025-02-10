namespace DaemonMC.Blocks
{
    public class AzaleaLeavesFlowered : Block
    {
        public AzaleaLeavesFlowered()
        {
            Name = "minecraft:azalea_leaves_flowered";

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
