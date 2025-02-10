namespace DaemonMC.Blocks
{
    public class UnlitRedstoneTorch : Block
    {
        public UnlitRedstoneTorch()
        {
            Name = "minecraft:unlit_redstone_torch";

            States["torch_facing_direction"] = "unknown";
        }
    }
}
