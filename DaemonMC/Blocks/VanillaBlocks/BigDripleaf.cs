namespace DaemonMC.Blocks
{
    public class BigDripleaf : Block
    {
        public BigDripleaf()
        {
            Name = "minecraft:big_dripleaf";

            States["big_dripleaf_head"] = (byte)0;
            States["big_dripleaf_tilt"] = "none";
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
