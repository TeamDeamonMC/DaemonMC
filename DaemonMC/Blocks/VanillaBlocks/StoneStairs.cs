namespace DaemonMC.Blocks
{
    public class StoneStairs : Block
    {
        public StoneStairs()
        {
            Name = "minecraft:stone_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
