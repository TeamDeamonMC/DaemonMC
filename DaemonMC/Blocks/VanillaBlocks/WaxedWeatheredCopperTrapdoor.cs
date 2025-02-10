namespace DaemonMC.Blocks
{
    public class WaxedWeatheredCopperTrapdoor : Block
    {
        public WaxedWeatheredCopperTrapdoor()
        {
            Name = "minecraft:waxed_weathered_copper_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
