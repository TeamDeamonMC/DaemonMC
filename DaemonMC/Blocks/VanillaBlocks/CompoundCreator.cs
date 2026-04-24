namespace DaemonMC.Blocks
{
    public class CompoundCreator : Block
    {
        public CompoundCreator()
        {
            Name = "minecraft:compound_creator";

            BlastResistance = 2.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2.5;
            Opacity = 1;

            States["direction"] = 0;
        }
    }
}
