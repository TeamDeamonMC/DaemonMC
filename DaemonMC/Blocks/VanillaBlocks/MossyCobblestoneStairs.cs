namespace DaemonMC.Blocks
{
    public class MossyCobblestoneStairs : Block
    {
        public MossyCobblestoneStairs()
        {
            Name = "minecraft:mossy_cobblestone_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
