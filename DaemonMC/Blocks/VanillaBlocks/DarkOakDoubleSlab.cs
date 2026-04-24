namespace DaemonMC.Blocks
{
    public class DarkOakDoubleSlab : Block
    {
        public DarkOakDoubleSlab()
        {
            Name = "minecraft:dark_oak_double_slab";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}
