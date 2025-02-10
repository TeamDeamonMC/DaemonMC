namespace DaemonMC.Blocks
{
    public class CrimsonStairs : Block
    {
        public CrimsonStairs()
        {
            Name = "minecraft:crimson_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
