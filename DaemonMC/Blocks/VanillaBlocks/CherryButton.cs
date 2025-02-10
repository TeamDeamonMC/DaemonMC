namespace DaemonMC.Blocks
{
    public class CherryButton : Block
    {
        public CherryButton()
        {
            Name = "minecraft:cherry_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
