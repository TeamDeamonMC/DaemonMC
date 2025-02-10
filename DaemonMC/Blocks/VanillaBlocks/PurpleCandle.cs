namespace DaemonMC.Blocks
{
    public class PurpleCandle : Block
    {
        public PurpleCandle()
        {
            Name = "minecraft:purple_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
