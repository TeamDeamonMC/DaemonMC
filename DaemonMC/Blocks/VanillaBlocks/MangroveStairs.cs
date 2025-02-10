namespace DaemonMC.Blocks
{
    public class MangroveStairs : Block
    {
        public MangroveStairs()
        {
            Name = "minecraft:mangrove_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
