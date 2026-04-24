namespace DaemonMC.Blocks
{
    public class WeatheredDoubleCutCopperSlab : Block
    {
        public WeatheredDoubleCutCopperSlab()
        {
            Name = "minecraft:weathered_double_cut_copper_slab";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}
