namespace DaemonMC.Blocks
{
    public class IronChain : Block
    {
        public IronChain()
        {
            Name = "minecraft:iron_chain";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 5;
            Opacity = 0.19999998807907104;

            States["pillar_axis"] = "y";
        }
    }
}
