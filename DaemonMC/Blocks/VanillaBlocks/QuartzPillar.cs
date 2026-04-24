namespace DaemonMC.Blocks
{
    public class QuartzPillar : Block
    {
        public QuartzPillar()
        {
            Name = "minecraft:quartz_pillar";

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
