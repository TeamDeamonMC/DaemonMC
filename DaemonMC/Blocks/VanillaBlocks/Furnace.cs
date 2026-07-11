namespace DaemonMC.Blocks
{
    public class Furnace : Block
    {
        public Furnace()
        {
            Name = "minecraft:furnace";

            BlastResistance = 3.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3.5;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
