namespace DaemonMC.Blocks
{
    public class LitPumpkin : Block
    {
        public LitPumpkin()
        {
            Name = "minecraft:lit_pumpkin";

            BlastResistance = 1;
            Brightness = 15;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
