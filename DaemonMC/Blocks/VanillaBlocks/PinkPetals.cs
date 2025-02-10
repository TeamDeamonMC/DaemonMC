namespace DaemonMC.Blocks
{
    public class PinkPetals : Block
    {
        public PinkPetals()
        {
            Name = "minecraft:pink_petals";

            States["growth"] = 0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
