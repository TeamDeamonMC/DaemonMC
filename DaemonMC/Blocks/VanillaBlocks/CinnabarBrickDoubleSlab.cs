namespace DaemonMC.Blocks
{
    public class CinnabarBrickDoubleSlab : Block
    {
        public CinnabarBrickDoubleSlab()
        {
            Name = "minecraft:cinnabar_brick_double_slab";

            BlastResistance = 3;
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
