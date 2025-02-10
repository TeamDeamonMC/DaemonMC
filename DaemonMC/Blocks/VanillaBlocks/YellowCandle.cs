namespace DaemonMC.Blocks
{
    public class YellowCandle : Block
    {
        public YellowCandle()
        {
            Name = "minecraft:yellow_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
