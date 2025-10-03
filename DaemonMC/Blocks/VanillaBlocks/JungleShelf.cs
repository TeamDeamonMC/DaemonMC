namespace DaemonMC.Blocks
{
    public class JungleShelf : Block
    {
        public JungleShelf()
        {
            Name = "minecraft:jungle_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
