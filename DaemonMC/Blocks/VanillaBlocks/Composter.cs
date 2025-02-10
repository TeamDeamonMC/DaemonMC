namespace DaemonMC.Blocks
{
    public class Composter : Block
    {
        public Composter()
        {
            Name = "minecraft:composter";

            States["composter_fill_level"] = 0;
        }
    }
}
