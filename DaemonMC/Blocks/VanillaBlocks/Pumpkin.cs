namespace DaemonMC.Blocks
{
    public class Pumpkin : Block
    {
        public Pumpkin()
        {
            Name = "minecraft:pumpkin";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
