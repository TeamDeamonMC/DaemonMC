namespace DaemonMC.Blocks
{
    public class RedstoneTorch : Block
    {
        public RedstoneTorch()
        {
            Name = "minecraft:redstone_torch";

            BlastResistance = 0;
            Brightness = 7;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["torch_facing_direction"] = "unknown";
        }
    }
}
