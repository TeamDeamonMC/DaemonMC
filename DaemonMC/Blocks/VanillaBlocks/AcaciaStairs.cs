namespace DaemonMC.Blocks
{
    public class AcaciaStairs : Block
    {
        public AcaciaStairs()
        {
            Name = "minecraft:acacia_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
