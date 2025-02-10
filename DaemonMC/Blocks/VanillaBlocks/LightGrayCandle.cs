namespace DaemonMC.Blocks
{
    public class LightGrayCandle : Block
    {
        public LightGrayCandle()
        {
            Name = "minecraft:light_gray_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
