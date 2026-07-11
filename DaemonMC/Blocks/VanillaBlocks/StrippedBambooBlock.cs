namespace DaemonMC.Blocks
{
    public class StrippedBambooBlock : Block
    {
        public StrippedBambooBlock()
        {
            Name = "minecraft:stripped_bamboo_block";

            BlastResistance = 2;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
