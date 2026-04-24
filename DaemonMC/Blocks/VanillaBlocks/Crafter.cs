namespace DaemonMC.Blocks
{
    public class Crafter : Block
    {
        public Crafter()
        {
            Name = "minecraft:crafter";

            BlastResistance = 3.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["crafting"] = (byte)0;
            States["orientation"] = "down_east";
            States["triggered_bit"] = (byte)0;
        }
    }
}
