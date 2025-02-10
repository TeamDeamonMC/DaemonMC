namespace DaemonMC.Blocks
{
    public class Hopper : Block
    {
        public Hopper()
        {
            Name = "minecraft:hopper";

            States["facing_direction"] = 0;
            States["toggle_bit"] = (byte)0;
        }
    }
}
