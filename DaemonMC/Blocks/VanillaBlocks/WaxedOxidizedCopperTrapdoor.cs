namespace DaemonMC.Blocks
{
    public class WaxedOxidizedCopperTrapdoor : Block
    {
        public WaxedOxidizedCopperTrapdoor()
        {
            Name = "minecraft:waxed_oxidized_copper_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
