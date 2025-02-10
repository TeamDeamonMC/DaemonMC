namespace DaemonMC.Blocks
{
    public class BambooMosaicStairs : Block
    {
        public BambooMosaicStairs()
        {
            Name = "minecraft:bamboo_mosaic_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
