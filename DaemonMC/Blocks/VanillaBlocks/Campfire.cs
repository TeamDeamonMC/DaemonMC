namespace DaemonMC.Blocks
{
    public class Campfire : Block
    {
        public Campfire()
        {
            Name = "minecraft:campfire";

            States["extinguished"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
