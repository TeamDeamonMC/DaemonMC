namespace DaemonMC.Blocks
{
    public class Chest : Block
    {
        public Chest()
        {
            Name = "minecraft:chest";

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
