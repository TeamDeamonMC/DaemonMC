namespace DaemonMC.Blocks
{
    public class CherryShelf : Block
    {
        public CherryShelf()
        {
            Name = "minecraft:cherry_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
