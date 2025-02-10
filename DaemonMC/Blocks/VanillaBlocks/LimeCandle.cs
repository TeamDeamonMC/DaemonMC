namespace DaemonMC.Blocks
{
    public class LimeCandle : Block
    {
        public LimeCandle()
        {
            Name = "minecraft:lime_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
