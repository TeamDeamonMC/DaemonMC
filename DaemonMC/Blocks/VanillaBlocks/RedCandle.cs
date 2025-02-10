namespace DaemonMC.Blocks
{
    public class RedCandle : Block
    {
        public RedCandle()
        {
            Name = "minecraft:red_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
