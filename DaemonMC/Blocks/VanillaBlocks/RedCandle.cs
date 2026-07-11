namespace DaemonMC.Blocks
{
    public class RedCandle : Block
    {
        public RedCandle()
        {
            Name = "minecraft:red_candle";

            BlastResistance = 0.10000000149011612;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.10000000149011612;
            Opacity = 0;

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
