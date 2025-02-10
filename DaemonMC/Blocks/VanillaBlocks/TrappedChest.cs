namespace DaemonMC.Blocks
{
    public class TrappedChest : Block
    {
        public TrappedChest()
        {
            Name = "minecraft:trapped_chest";


            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
