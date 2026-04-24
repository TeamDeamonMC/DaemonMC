namespace DaemonMC.Blocks
{
    public class ColoredTorchPurple : Block
    {
        public ColoredTorchPurple()
        {
            Name = "minecraft:colored_torch_purple";

            BlastResistance = 0;
            Brightness = 14;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["torch_facing_direction"] = "unknown";
        }
    }
}
