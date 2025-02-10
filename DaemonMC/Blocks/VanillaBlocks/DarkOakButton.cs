namespace DaemonMC.Blocks
{
    public class DarkOakButton : Block
    {
        public DarkOakButton()
        {
            Name = "minecraft:dark_oak_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
