namespace DaemonMC.Blocks
{
    public class JungleShelf : Block
    {
        public JungleShelf()
        {
            Name = "minecraft:jungle_shelf";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 30;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 0.19999998807907104;

            States["minecraft:cardinal_direction"] = "south";
            States["powered_bit"] = (byte)0;
            States["powered_shelf_type"] = 0;
        }
    }
}
