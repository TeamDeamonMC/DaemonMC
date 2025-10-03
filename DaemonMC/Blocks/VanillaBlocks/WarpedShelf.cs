namespace DaemonMC.Blocks
{
    public class WarpedShelf : Block
    {
        public WarpedShelf()
        {
            Name = "minecraft:warped_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
