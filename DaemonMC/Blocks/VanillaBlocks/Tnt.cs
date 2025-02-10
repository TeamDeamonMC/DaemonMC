namespace DaemonMC.Blocks
{
    public class Tnt : Block
    {
        public Tnt()
        {
            Name = "minecraft:tnt";


            States["explode_bit"] = (byte)0;
        }
    }
}
