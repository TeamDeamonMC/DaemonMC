namespace DaemonMC.Blocks
{
    public class OxidizedCopperTrapdoor : Block
    {
        public OxidizedCopperTrapdoor()
        {
            Name = "minecraft:oxidized_copper_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
