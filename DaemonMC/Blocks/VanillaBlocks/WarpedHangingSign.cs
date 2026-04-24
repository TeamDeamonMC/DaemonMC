namespace DaemonMC.Blocks
{
    public class WarpedHangingSign : Block
    {
        public WarpedHangingSign()
        {
            Name = "minecraft:warped_hanging_sign";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0.19999998807907104;

            States["attached_bit"] = (byte)0;
            States["facing_direction"] = 0;
            States["ground_sign_direction"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
