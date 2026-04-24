namespace DaemonMC.Blocks
{
    public class EndStoneBrickSlab : Block
    {
        public EndStoneBrickSlab()
        {
            Name = "minecraft:end_stone_brick_slab";

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
