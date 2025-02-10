namespace DaemonMC.Blocks
{
    public class Smoker : Block
    {
        public Smoker()
        {
            Name = "minecraft:smoker";

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
