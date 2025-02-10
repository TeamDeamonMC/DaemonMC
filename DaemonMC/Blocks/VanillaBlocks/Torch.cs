namespace DaemonMC.Blocks
{
    public class Torch : Block
    {
        public Torch()
        {
            Name = "minecraft:torch";

            States["torch_facing_direction"] = "unknown";
        }
    }
}
