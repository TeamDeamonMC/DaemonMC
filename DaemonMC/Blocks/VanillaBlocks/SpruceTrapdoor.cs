namespace DaemonMC.Blocks
{
    public class SpruceTrapdoor : Block
    {
        public SpruceTrapdoor()
        {
            Name = "minecraft:spruce_trapdoor";

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
