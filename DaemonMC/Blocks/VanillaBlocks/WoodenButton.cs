namespace DaemonMC.Blocks
{
    public class WoodenButton : Block
    {
        public WoodenButton()
        {
            Name = "minecraft:wooden_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
