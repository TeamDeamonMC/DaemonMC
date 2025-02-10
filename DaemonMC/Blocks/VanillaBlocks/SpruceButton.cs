namespace DaemonMC.Blocks
{
    public class SpruceButton : Block
    {
        public SpruceButton()
        {
            Name = "minecraft:spruce_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
