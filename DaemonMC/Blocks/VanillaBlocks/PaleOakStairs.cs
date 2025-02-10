namespace DaemonMC.Blocks
{
    public class PaleOakStairs : Block
    {
        public PaleOakStairs()
        {
            Name = "minecraft:pale_oak_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
