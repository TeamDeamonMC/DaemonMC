namespace DaemonMC.Blocks
{
    public class HayBlock : Block
    {
        public HayBlock()
        {
            Name = "minecraft:hay_block";

            States["deprecated"] = 0;
            States["pillar_axis"] = "y";
        }
    }
}
