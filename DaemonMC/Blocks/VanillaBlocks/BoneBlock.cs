namespace DaemonMC.Blocks
{
    public class BoneBlock : Block
    {
        public BoneBlock()
        {
            Name = "minecraft:bone_block";

            States["deprecated"] = 0;
            States["pillar_axis"] = "y";
        }
    }
}
