namespace DaemonMC.Blocks
{
    public class Barrel : Block
    {
        public Barrel()
        {
            Name = "minecraft:barrel";

            States["facing_direction"] = 0;
            States["open_bit"] = (byte)0;
        }
    }
}
