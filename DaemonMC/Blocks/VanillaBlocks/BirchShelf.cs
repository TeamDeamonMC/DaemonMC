namespace DaemonMC.Blocks
{
    public class BirchShelf : Block
    {
        public BirchShelf()
        {
            Name = "minecraft:birch_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
