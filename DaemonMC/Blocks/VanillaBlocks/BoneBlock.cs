namespace DaemonMC.Blocks
{
    public class BoneBlock : Block
    {
        public BoneBlock()
        {
            Name = "minecraft:bone_block";

            BlastResistance = 2;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["deprecated"] = 0;
            States["pillar_axis"] = "y";
        }
    }
}
