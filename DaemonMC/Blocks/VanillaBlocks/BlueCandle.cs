namespace DaemonMC.Blocks
{
    public class BlueCandle : Block
    {
        public BlueCandle()
        {
            Name = "minecraft:blue_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
