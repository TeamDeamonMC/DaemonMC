namespace DaemonMC.Blocks
{
    public class PolishedDioriteStairs : Block
    {
        public PolishedDioriteStairs()
        {
            Name = "minecraft:polished_diorite_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
