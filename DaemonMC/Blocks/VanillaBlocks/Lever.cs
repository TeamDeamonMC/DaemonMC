namespace DaemonMC.Blocks
{
    public class Lever : Block
    {
        public Lever()
        {
            Name = "minecraft:lever";

            States["lever_direction"] = "down_east_west";
            States["open_bit"] = (byte)0;
        }
    }
}
