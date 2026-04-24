namespace DaemonMC.Blocks
{
    public class PrismarineDoubleSlab : Block
    {
        public PrismarineDoubleSlab()
        {
            Name = "minecraft:prismarine_double_slab";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}
