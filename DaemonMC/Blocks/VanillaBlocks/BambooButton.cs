namespace DaemonMC.Blocks
{
    public class BambooButton : Block
    {
        public BambooButton()
        {
            Name = "minecraft:bamboo_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
