namespace DaemonMC.Blocks
{
    public class WaxedExposedCopperTrapdoor : Block
    {
        public WaxedExposedCopperTrapdoor()
        {
            Name = "minecraft:waxed_exposed_copper_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
