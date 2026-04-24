namespace DaemonMC.Blocks
{
    public class Cauldron : Block
    {
        public Cauldron()
        {
            Name = "minecraft:cauldron";

            BlastResistance = 2;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 0.19999998807907104;

            States["cauldron_liquid"] = "water";
            States["fill_level"] = 0;
        }
    }
}
