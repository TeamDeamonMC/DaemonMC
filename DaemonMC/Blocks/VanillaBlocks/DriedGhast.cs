namespace DaemonMC.Blocks
{
    public class DriedGhast : Block
    {
        public DriedGhast()
        {
            Name = "minecraft:dried_ghast";

            States["minecraft:cardinal_direction"] = "south";
            States["rehydration_level"] = 0;
        }
    }
}
