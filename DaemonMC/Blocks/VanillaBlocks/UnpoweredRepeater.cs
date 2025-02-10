namespace DaemonMC.Blocks
{
    public class UnpoweredRepeater : Block
    {
        public UnpoweredRepeater()
        {
            Name = "minecraft:unpowered_repeater";

            States["minecraft:cardinal_direction"] = "south";
            States["repeater_delay"] = 0;
        }
    }
}
