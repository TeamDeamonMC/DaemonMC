namespace DaemonMC.Blocks
{
    public class StructureBlock : Block
    {
        public StructureBlock()
        {
            Name = "minecraft:structure_block";

            BlastResistance = 3600000;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = -1;
            Opacity = 1;

            States["structure_block_type"] = "data";
        }
    }
}
