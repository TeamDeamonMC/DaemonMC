namespace DaemonMC.Blocks
{
    public class Candle : Block
    {
        public Candle()
        {
            Name = "minecraft:candle";

            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
