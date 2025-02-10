namespace DaemonMC.Blocks
{
    public class Beehive : Block
    {
        public Beehive()
        {
            Name = "minecraft:beehive";


            States["direction"] = 0;
            States["honey_level"] = 0;
        }
    }
}
