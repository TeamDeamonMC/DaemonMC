namespace DaemonMC.Blocks
{
    public class SoulCampfire : Block
    {
        public SoulCampfire()
        {
            Name = "minecraft:soul_campfire";


            States["extinguished"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
