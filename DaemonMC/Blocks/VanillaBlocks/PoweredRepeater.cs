namespace DaemonMC.Blocks
{
    public class PoweredRepeater : Block
    {
        public PoweredRepeater()
        {
            Name = "minecraft:powered_repeater";


            States["minecraft:cardinal_direction"] = "south";
            States["repeater_delay"] = 0;
        }
    }
}
