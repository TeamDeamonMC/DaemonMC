namespace DaemonMC.Blocks
{
    public class BlackstoneStairs : Block
    {
        public BlackstoneStairs()
        {
            Name = "minecraft:blackstone_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
