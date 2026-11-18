namespace DaemonMC.Blocks
{
    public class Icicle : Block
    {
        public Icicle()
        {
            Name = "minecraft:icicle";


            States["attached_bit"] = (byte)0;
            States["dripstone_thickness"] = "tip";
            States["hanging"] = (byte)0;
        }
    }
}
