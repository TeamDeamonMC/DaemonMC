namespace DaemonMC.Blocks
{
    public class DragonHead : Block
    {
        public DragonHead()
        {
            Name = "minecraft:dragon_head";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0;

            States["facing_direction"] = 0;
        }
    }
}
