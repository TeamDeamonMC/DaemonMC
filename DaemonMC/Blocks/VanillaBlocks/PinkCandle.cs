namespace DaemonMC.Blocks
{
    public class PinkCandle : Block
    {
        public PinkCandle()
        {
            Name = "minecraft:pink_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
