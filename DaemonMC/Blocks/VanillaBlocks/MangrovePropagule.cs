namespace DaemonMC.Blocks
{
    public class MangrovePropagule : Block
    {
        public MangrovePropagule()
        {
            Name = "minecraft:mangrove_propagule";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["hanging"] = (byte)0;
            States["propagule_stage"] = 0;
        }
    }
}
