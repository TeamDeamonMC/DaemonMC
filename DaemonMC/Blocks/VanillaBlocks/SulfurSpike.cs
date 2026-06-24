namespace DaemonMC.Blocks
{
    public class SulfurSpike : Block
    {
        public SulfurSpike()
        {
            Name = "minecraft:sulfur_spike";


            States["dripstone_thickness"] = "tip";
            States["hanging"] = (byte)0;
        }
    }
}
