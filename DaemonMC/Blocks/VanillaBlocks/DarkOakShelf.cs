namespace DaemonMC.Blocks
{
    public class DarkOakShelf : Block
    {
        public DarkOakShelf()
        {
            Name = "minecraft:dark_oak_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
