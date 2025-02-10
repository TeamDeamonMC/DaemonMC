namespace DaemonMC.Blocks
{
    public class RedstoneTorch : Block
    {
        public RedstoneTorch()
        {
            Name = "minecraft:redstone_torch";

            States["torch_facing_direction"] = "unknown";
        }
    }
}
