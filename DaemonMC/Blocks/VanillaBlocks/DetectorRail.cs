namespace DaemonMC.Blocks
{
    public class DetectorRail : Block
    {
        public DetectorRail()
        {
            Name = "minecraft:detector_rail";

            BlastResistance = 0.699999988079071;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.699999988079071;
            Opacity = 0;

            States["rail_data_bit"] = (byte)0;
            States["rail_direction"] = 0;
        }
    }
}
