namespace DaemonMC.Blocks
{
    public class LeafLitter : Block
    {
        public LeafLitter()
        {
            Name = "minecraft:leaf_litter";

            States["growth"] = 0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
