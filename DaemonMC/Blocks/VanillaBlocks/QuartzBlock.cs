namespace DaemonMC.Blocks
{
    public class QuartzBlock : Block
    {
        public QuartzBlock()
        {
            Name = "minecraft:quartz_block";

            BlastResistance = 0.800000011920929;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.800000011920929;
            Opacity = 1;

            States["pillar_axis"] = "y";
        }
    }
}
