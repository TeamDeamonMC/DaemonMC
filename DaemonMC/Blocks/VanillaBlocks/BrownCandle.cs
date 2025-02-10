namespace DaemonMC.Blocks
{
    public class BrownCandle : Block
    {
        public BrownCandle()
        {
            Name = "minecraft:brown_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
