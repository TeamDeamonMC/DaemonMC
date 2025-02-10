namespace DaemonMC.Blocks
{
    public class FlowingWater : Block
    {
        public FlowingWater()
        {
            Name = "minecraft:flowing_water";

            States["liquid_depth"] = 0;
        }
    }
}
