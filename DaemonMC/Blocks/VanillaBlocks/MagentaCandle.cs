namespace DaemonMC.Blocks
{
    public class MagentaCandle : Block
    {
        public MagentaCandle()
        {
            Name = "minecraft:magenta_candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
