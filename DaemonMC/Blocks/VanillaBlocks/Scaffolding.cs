namespace DaemonMC.Blocks
{
    public class Scaffolding : Block
    {
        public Scaffolding()
        {
            Name = "minecraft:scaffolding";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 60;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["stability"] = 0;
            States["stability_check"] = (byte)0;
        }
    }
}
