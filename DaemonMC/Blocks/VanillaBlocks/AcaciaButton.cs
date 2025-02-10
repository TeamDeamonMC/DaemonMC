namespace DaemonMC.Blocks
{
    public class AcaciaButton : Block
    {
        public AcaciaButton()
        {
            Name = "minecraft:acacia_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
