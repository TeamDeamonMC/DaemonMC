namespace DaemonMC.Blocks
{
    public class CherryStairs : Block
    {
        public CherryStairs()
        {
            Name = "minecraft:cherry_stairs";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 0.19999998807907104;

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
