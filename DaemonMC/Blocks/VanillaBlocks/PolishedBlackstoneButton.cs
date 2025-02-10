namespace DaemonMC.Blocks
{
    public class PolishedBlackstoneButton : Block
    {
        public PolishedBlackstoneButton()
        {
            Name = "minecraft:polished_blackstone_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
