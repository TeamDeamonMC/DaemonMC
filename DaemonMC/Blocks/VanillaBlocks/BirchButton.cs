namespace DaemonMC.Blocks
{
    public class BirchButton : Block
    {
        public BirchButton()
        {
            Name = "minecraft:birch_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
