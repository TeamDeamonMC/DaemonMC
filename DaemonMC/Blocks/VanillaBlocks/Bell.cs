namespace DaemonMC.Blocks
{
    public class Bell : Block
    {
        public Bell()
        {
            Name = "minecraft:bell";

            States["attachment"] = "standing";
            States["direction"] = 0;
            States["toggle_bit"] = (byte)0;
        }
    }
}
