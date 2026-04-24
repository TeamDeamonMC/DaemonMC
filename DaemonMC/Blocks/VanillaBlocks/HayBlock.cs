namespace DaemonMC.Blocks
{
    public class HayBlock : Block
    {
        public HayBlock()
        {
            Name = "minecraft:hay_block";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 60;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 1;

            States["deprecated"] = 0;
            States["pillar_axis"] = "y";
        }
    }
}
