namespace DaemonMC.Blocks
{
    public class AcaciaShelf : Block
    {
        public AcaciaShelf()
        {
            Name = "minecraft:acacia_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
