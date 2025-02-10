namespace DaemonMC.Blocks
{
    public class BlackCandle : Block
    {
        public BlackCandle()
        {
            Name = "minecraft:black_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
