namespace DaemonMC.Blocks
{
    public class Observer : Block
    {
        public Observer()
        {
            Name = "minecraft:observer";

            States["minecraft:facing_direction"] = "down";
            States["powered_bit"] = (byte)0;
        }
    }
}
