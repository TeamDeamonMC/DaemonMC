namespace DaemonMC.Blocks
{
    public class WhiteCandle : Block
    {
        public WhiteCandle()
        {
            Name = "minecraft:white_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
