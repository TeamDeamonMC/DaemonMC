namespace DaemonMC.Blocks
{
    public class FlowingLava : Block
    {
        public FlowingLava()
        {
            Name = "minecraft:flowing_lava";

            BlastResistance = 100;
            Brightness = 15;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 100;
            Opacity = 0;

            States["liquid_depth"] = 0;
        }
    }
}
