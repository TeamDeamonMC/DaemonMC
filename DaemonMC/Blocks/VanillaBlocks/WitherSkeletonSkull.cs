namespace DaemonMC.Blocks
{
    public class WitherSkeletonSkull : Block
    {
        public WitherSkeletonSkull()
        {
            Name = "minecraft:wither_skeleton_skull";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0;

            States["facing_direction"] = 0;
        }
    }
}
