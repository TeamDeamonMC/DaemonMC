namespace DaemonMC.Blocks
{
    public class Crafter : Block
    {
        public Crafter()
        {
            Name = "minecraft:crafter";

            States["crafting"] = (byte)0;
            States["orientation"] = "down_east";
            States["triggered_bit"] = (byte)0;
        }
    }
}
