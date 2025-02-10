namespace DaemonMC.Blocks
{
    public class SuspiciousSand : Block
    {
        public SuspiciousSand()
        {
            Name = "minecraft:suspicious_sand";

            States["brushed_progress"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
