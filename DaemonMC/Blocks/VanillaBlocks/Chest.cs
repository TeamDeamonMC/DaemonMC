namespace DaemonMC.Blocks
{
    public class Chest : Block
    {
        public Chest()
        {
            Name = "minecraft:chest";

            BlastResistance = 2.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2.5;
            Opacity = 0.5;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
