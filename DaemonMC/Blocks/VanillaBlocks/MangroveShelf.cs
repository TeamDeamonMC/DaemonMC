namespace DaemonMC.Blocks
{
    public class MangroveShelf : Block
    {
        public MangroveShelf()
        {
            Name = "minecraft:mangrove_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
