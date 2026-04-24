namespace DaemonMC.Blocks
{
    public class CherryButton : Block
    {
        public CherryButton()
        {
            Name = "minecraft:cherry_button";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 1;

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
