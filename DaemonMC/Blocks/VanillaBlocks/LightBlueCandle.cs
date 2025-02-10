namespace DaemonMC.Blocks
{
    public class LightBlueCandle : Block
    {
        public LightBlueCandle()
        {
            Name = "minecraft:light_blue_candle";


            States["candles"] = 0;
            States["lit"] = (byte)0;
        }
    }
}
