namespace DaemonMC.Blocks
{
    public class AcaciaTrapdoor : Block
    {
        public AcaciaTrapdoor()
        {
            Name = "minecraft:acacia_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
