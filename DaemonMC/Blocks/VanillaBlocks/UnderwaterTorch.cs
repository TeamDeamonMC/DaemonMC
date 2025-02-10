namespace DaemonMC.Blocks
{
    public class UnderwaterTorch : Block
    {
        public UnderwaterTorch()
        {
            Name = "minecraft:underwater_torch";

            States["torch_facing_direction"] = "unknown";
        }
    }
}
