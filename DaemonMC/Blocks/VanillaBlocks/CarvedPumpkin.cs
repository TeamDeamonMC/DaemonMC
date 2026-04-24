namespace DaemonMC.Blocks
{
    public class CarvedPumpkin : Block
    {
        public CarvedPumpkin()
        {
            Name = "minecraft:carved_pumpkin";

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
