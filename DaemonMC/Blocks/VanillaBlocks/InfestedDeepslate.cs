namespace DaemonMC.Blocks
{
    public class InfestedDeepslate : Block
    {
        public InfestedDeepslate()
        {
            Name = "minecraft:infested_deepslate";

            BlastResistance = 0.75;
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
