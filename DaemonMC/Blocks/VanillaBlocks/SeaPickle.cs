namespace DaemonMC.Blocks
{
    public class SeaPickle : Block
    {
        public SeaPickle()
        {
            Name = "minecraft:sea_pickle";


            States["cluster_count"] = 0;
            States["dead_bit"] = (byte)0;
        }
    }
}
