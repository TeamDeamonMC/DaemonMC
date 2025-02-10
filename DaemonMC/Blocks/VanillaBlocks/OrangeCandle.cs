namespace DaemonMC.Blocks
{
    public class OrangeCandle : Block
    {
        public OrangeCandle()
        {
            Name = "minecraft:orange_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
