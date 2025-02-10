namespace DaemonMC.Blocks
{
    public class Grindstone : Block
    {
        public Grindstone()
        {
            Name = "minecraft:grindstone";


            States["attachment"] = "standing";
            States["direction"] = 0;
        }
    }
}
