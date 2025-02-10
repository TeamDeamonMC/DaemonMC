namespace DaemonMC.Blocks
{
    public class WarpedButton : Block
    {
        public WarpedButton()
        {
            Name = "minecraft:warped_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
