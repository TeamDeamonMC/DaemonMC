namespace DaemonMC.Blocks
{
    public class MudBrickStairs : Block
    {
        public MudBrickStairs()
        {
            Name = "minecraft:mud_brick_stairs";


            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}
