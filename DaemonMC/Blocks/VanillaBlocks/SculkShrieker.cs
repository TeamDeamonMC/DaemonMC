namespace DaemonMC.Blocks
{
    public class SculkShrieker : Block
    {
        public SculkShrieker()
        {
            Name = "minecraft:sculk_shrieker";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 0.19999998807907104;

            States["active"] = (byte)0;
            States["can_summon"] = (byte)0;
        }
    }
}
