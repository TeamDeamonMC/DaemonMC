namespace DaemonMC.Blocks
{
    public class Lectern : Block
    {
        public Lectern()
        {
            Name = "minecraft:lectern";


            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
        }
    }
}
