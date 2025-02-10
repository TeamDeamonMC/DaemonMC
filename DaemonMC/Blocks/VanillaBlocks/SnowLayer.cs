namespace DaemonMC.Blocks
{
    public class SnowLayer : Block
    {
        public SnowLayer()
        {
            Name = "minecraft:snow_layer";

            States["covered_bit"] = (byte)0;
            States["height"] = 0;
        }
    }
}
