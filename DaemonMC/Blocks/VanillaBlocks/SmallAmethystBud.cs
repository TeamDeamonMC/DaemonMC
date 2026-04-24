namespace DaemonMC.Blocks
{
    public class SmallAmethystBud : Block
    {
        public SmallAmethystBud()
        {
            Name = "minecraft:small_amethyst_bud";

            BlastResistance = 1.5;
            Brightness = 1;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["minecraft:block_face"] = "down";
        }
    }
}
