namespace DaemonMC.Blocks
{
    public class CreakingHeart : Block
    {
        public CreakingHeart()
        {
            Name = "minecraft:creaking_heart";

            States["creaking_heart_state"] = "uprooted";
            States["natural"] = (byte)0;
            States["pillar_axis"] = "y";
        }
    }
}
