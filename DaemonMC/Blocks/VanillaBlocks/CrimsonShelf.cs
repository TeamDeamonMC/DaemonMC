namespace DaemonMC.Blocks
{
    public class CrimsonShelf : Block
    {
        public CrimsonShelf()
        {
            Name = "minecraft:crimson_shelf";

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
