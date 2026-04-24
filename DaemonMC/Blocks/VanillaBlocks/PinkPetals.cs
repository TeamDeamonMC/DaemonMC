namespace DaemonMC.Blocks
{
    public class PinkPetals : Block
    {
        public PinkPetals()
        {
            Name = "minecraft:pink_petals";

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
