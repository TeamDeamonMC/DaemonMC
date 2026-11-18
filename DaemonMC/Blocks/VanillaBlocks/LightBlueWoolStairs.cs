namespace DaemonMC.Blocks
{
    public class LightBlueWoolStairs : Block
    {
        public LightBlueWoolStairs()
        {
            Name = "minecraft:light_blue_wool_stairs";

            BlastResistance = 0.1599999964237213;
            Brightness = 0;
            FlameEncouragement = 30;
            Flammability = 60;
            Friction = 0.6000000238418579;
            Hardness = 0.800000011920929;
            Opacity = 0.19999998807907104;

            States["minecraft:corner"] = "none";
            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
