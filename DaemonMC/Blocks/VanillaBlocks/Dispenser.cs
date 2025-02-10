namespace DaemonMC.Blocks
{
    public class Dispenser : Block
    {
        public Dispenser()
        {
            Name = "minecraft:dispenser";

            States["facing_direction"] = 0;
            States["triggered_bit"] = (byte)0;
        }
    }
}
