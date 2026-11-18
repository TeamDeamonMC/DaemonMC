namespace DaemonMC.Blocks
{
    public class PurpleWoolDoubleSlab : Block
    {
        public PurpleWoolDoubleSlab()
        {
            Name = "minecraft:purple_wool_double_slab";

            BlastResistance = 0.1599999964237213;
            Brightness = 0;
            FlameEncouragement = 30;
            Flammability = 60;
            Friction = 0.6000000238418579;
            Hardness = 0.800000011920929;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}
