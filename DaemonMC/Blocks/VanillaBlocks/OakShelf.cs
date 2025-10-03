namespace DaemonMC.Blocks
{
    public class OakShelf : Block
    {
        public OakShelf()
        {
            Name = "minecraft:oak_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
