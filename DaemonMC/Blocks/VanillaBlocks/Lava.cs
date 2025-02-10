namespace DaemonMC.Blocks
{
    public class Lava : Block
    {
        public Lava()
        {
            Name = "minecraft:lava";

            States["liquid_depth"] = 0;
        }
    }
}
