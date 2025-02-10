namespace DaemonMC.Blocks
{
    public class GrayCandle : Block
    {
        public GrayCandle()
        {
            Name = "minecraft:gray_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
