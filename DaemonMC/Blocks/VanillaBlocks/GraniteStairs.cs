namespace DaemonMC.Blocks
{
    public class GraniteStairs : Block
    {
        public GraniteStairs()
        {
            Name = "minecraft:granite_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
