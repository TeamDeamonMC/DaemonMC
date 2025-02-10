namespace DaemonMC.Blocks
{
    public class PointedDripstone : Block
    {
        public PointedDripstone()
        {
            Name = "minecraft:pointed_dripstone";


            States["dripstone_thickness"] = "tip";
            States["hanging"] = (byte)0;
        }
    }
}
