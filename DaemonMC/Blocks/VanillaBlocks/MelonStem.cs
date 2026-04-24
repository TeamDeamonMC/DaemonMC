namespace DaemonMC.Blocks
{
    public class MelonStem : Block
    {
        public MelonStem()
        {
            Name = "minecraft:melon_stem";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["facing_direction"] = 0;
            States["growth"] = 0;
        }
    }
}
