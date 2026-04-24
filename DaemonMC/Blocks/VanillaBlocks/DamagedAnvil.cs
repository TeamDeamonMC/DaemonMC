namespace DaemonMC.Blocks
{
    public class DamagedAnvil : Block
    {
        public DamagedAnvil()
        {
            Name = "minecraft:damaged_anvil";

            BlastResistance = 1200;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 5;
            Opacity = 0.19999998807907104;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
