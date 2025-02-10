namespace DaemonMC.Blocks
{
    public class JungleStairs : Block
    {
        public JungleStairs()
        {
            Name = "minecraft:jungle_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
