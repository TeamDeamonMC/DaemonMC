namespace DaemonMC.Blocks
{
    public class Farmland : Block
    {
        public Farmland()
        {
            Name = "minecraft:farmland";

            States["moisturized_amount"] = 0;
        }
    }
}
