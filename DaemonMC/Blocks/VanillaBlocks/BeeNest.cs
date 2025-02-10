namespace DaemonMC.Blocks
{
    public class BeeNest : Block
    {
        public BeeNest()
        {
            Name = "minecraft:bee_nest";

            States["direction"] = 0;
            States["honey_level"] = 0;
        }
    }
}
