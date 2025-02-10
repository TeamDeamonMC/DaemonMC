namespace DaemonMC.Blocks
{
    public class MangrovePropagule : Block
    {
        public MangrovePropagule()
        {
            Name = "minecraft:mangrove_propagule";

            States["hanging"] = (byte)0;
            States["propagule_stage"] = 0;
        }
    }
}
