namespace DaemonMC.Blocks
{
    public class DioriteStairs : Block
    {
        public DioriteStairs()
        {
            Name = "minecraft:diorite_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
