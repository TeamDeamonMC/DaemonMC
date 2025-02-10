namespace DaemonMC.Blocks
{
    public class SpruceStairs : Block
    {
        public SpruceStairs()
        {
            Name = "minecraft:spruce_stairs";

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
