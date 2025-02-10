namespace DaemonMC.Blocks
{
    public class ExposedCopperTrapdoor : Block
    {
        public ExposedCopperTrapdoor()
        {
            Name = "minecraft:exposed_copper_trapdoor";


            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
