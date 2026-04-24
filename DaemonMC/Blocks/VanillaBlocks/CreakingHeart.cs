namespace DaemonMC.Blocks
{
    public class CreakingHeart : Block
    {
        public CreakingHeart()
        {
            Name = "minecraft:creaking_heart";

            BlastResistance = 10;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 10;
            Opacity = 1;

            States["creaking_heart_state"] = "uprooted";
            States["natural"] = (byte)0;
            States["pillar_axis"] = "y";
        }
    }
}
