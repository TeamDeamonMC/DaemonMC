namespace DaemonMC.Blocks
{
    public class SpruceStandingSign : Block
    {
        public SpruceStandingSign()
        {
            Name = "minecraft:spruce_standing_sign";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0.19999998807907104;

            States["ground_sign_direction"] = 0;
        }
    }
}
