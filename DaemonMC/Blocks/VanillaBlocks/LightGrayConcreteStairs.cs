namespace DaemonMC.Blocks
{
    public class LightGrayConcreteStairs : Block
    {
        public LightGrayConcreteStairs()
        {
            Name = "minecraft:light_gray_concrete_stairs";

            BlastResistance = 0.35999998450279236;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.7999999523162842;
            Opacity = 0.19999998807907104;

            States["minecraft:corner"] = "none";
            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
