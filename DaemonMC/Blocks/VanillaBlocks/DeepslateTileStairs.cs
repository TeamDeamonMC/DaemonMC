namespace DaemonMC.Blocks
{
    public class DeepslateTileStairs : Block
    {
        public DeepslateTileStairs()
        {
            Name = "minecraft:deepslate_tile_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
