namespace DaemonMC.Blocks
{
    public class PaleOakShelf : Block
    {
        public PaleOakShelf()
        {
            Name = "minecraft:pale_oak_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
