namespace DaemonMC.Blocks
{
    public class BambooShelf : Block
    {
        public BambooShelf()
        {
            Name = "minecraft:bamboo_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
