namespace DaemonMC.Blocks
{
    public class WhiteCandleCake : Block
    {
        public WhiteCandleCake()
        {
            Name = "minecraft:white_candle_cake";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 0.19999998807907104;

            States["lit"] = (byte)0;
        }
    }
}
