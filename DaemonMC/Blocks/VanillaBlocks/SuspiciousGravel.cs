namespace DaemonMC.Blocks
{
    public class SuspiciousGravel : Block
    {
        public SuspiciousGravel()
        {
            Name = "minecraft:suspicious_gravel";

            States["brushed_progress"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
