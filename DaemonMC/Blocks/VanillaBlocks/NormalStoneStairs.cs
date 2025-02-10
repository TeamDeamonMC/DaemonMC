namespace DaemonMC.Blocks
{
    public class NormalStoneStairs : Block
    {
        public NormalStoneStairs()
        {
            Name = "minecraft:normal_stone_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
