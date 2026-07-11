namespace DaemonMC.Blocks
{
    public class SweetBerryBush : Block
    {
        public SweetBerryBush()
        {
            Name = "minecraft:sweet_berry_bush";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 60;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["growth"] = 0;
        }
    }
}
