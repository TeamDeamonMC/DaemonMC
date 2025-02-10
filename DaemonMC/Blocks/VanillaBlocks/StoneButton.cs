namespace DaemonMC.Blocks
{
    public class StoneButton : Block
    {
        public StoneButton()
        {
            Name = "minecraft:stone_button";

            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
