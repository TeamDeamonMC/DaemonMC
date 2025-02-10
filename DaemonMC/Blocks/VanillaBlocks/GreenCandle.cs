namespace DaemonMC.Blocks
{
    public class GreenCandle : Block
    {
        public GreenCandle()
        {
            Name = "minecraft:green_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
