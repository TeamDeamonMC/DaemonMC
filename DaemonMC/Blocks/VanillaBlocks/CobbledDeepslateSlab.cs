namespace DaemonMC.Blocks
{
    public class CobbledDeepslateSlab : Block
    {
        public CobbledDeepslateSlab()
        {
            Name = "minecraft:cobbled_deepslate_slab";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3.5;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}
