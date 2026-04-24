namespace DaemonMC.Blocks
{
    public class PolishedTuffDoubleSlab : Block
    {
        public PolishedTuffDoubleSlab()
        {
            Name = "minecraft:polished_tuff_double_slab";

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
