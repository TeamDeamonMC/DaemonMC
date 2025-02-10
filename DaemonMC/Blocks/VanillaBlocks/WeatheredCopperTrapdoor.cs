namespace DaemonMC.Blocks
{
    public class WeatheredCopperTrapdoor : Block
    {
        public WeatheredCopperTrapdoor()
        {
            Name = "minecraft:weathered_copper_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
