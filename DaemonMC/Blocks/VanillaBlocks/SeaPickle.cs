namespace DaemonMC.Blocks
{
    public class SeaPickle : Block
    {
        public SeaPickle()
        {
            Name = "minecraft:sea_pickle";

            BlastResistance = 0;
            Brightness = 6;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["cluster_count"] = 0;
            States["dead_bit"] = (byte)0;
        }
    }
}
