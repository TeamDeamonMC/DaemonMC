namespace DaemonMC.Blocks
{
    public class DriedGhast : Block
    {
        public DriedGhast()
        {
            Name = "minecraft:dried_ghast";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
            States["rehydration_level"] = 0;
        }
    }
}
