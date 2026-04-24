namespace DaemonMC.Blocks
{
    public class SmoothRedSandstoneSlab : Block
    {
        public SmoothRedSandstoneSlab()
        {
            Name = "minecraft:smooth_red_sandstone_slab";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}
