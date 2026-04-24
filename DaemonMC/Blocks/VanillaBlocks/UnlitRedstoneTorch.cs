namespace DaemonMC.Blocks
{
    public class UnlitRedstoneTorch : Block
    {
        public UnlitRedstoneTorch()
        {
            Name = "minecraft:unlit_redstone_torch";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["torch_facing_direction"] = "unknown";
        }
    }
}
