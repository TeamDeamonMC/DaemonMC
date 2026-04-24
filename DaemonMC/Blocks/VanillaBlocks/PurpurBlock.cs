namespace DaemonMC.Blocks
{
    public class PurpurBlock : Block
    {
        public PurpurBlock()
        {
            Name = "minecraft:purpur_block";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
