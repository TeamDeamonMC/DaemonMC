namespace DaemonMC.Blocks
{
    public class JungleButton : Block
    {
        public JungleButton()
        {
            Name = "minecraft:jungle_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
