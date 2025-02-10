namespace DaemonMC.Blocks
{
    public class Dropper : Block
    {
        public Dropper()
        {
            Name = "minecraft:dropper";


            States["facing_direction"] = 0;
            States["triggered_bit"] = (byte)0;
        }
    }
}
