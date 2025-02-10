namespace DaemonMC.Blocks
{
    public class MangroveButton : Block
    {
        public MangroveButton()
        {
            Name = "minecraft:mangrove_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
