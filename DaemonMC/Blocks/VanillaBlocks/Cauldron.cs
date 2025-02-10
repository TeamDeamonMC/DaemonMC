namespace DaemonMC.Blocks
{
    public class Cauldron : Block
    {
        public Cauldron()
        {
            Name = "minecraft:cauldron";

            States["cauldron_liquid"] = "water";
            States["fill_level"] = 0;
        }
    }
}
