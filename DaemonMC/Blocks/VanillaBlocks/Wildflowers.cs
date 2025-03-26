namespace DaemonMC.Blocks
{
    public class Wildflowers : Block
    {
        public Wildflowers()
        {
            Name = "minecraft:wildflowers";

            States["growth"] = 0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
