namespace DaemonMC.Blocks
{
    public class Wildflowers : Block
    {
        public Wildflowers()
        {
            Name = "minecraft:wildflowers";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 60;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["growth"] = 0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
