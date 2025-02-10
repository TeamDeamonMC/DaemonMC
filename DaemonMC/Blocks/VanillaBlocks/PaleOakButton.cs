namespace DaemonMC.Blocks
{
    public class PaleOakButton : Block
    {
        public PaleOakButton()
        {
            Name = "minecraft:pale_oak_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
