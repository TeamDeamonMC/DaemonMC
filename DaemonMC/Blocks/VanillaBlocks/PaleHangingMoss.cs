namespace DaemonMC.Blocks
{
    public class PaleHangingMoss : Block
    {
        public PaleHangingMoss()
        {
            Name = "minecraft:pale_hanging_moss";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["tip"] = (byte)0;
        }
    }
}
