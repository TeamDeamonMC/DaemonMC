namespace DaemonMC.Blocks
{
    public class CyanCandle : Block
    {
        public CyanCandle()
        {
            Name = "minecraft:cyan_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
