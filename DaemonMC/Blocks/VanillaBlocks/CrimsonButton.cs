namespace DaemonMC.Blocks
{
    public class CrimsonButton : Block
    {
        public CrimsonButton()
        {
            Name = "minecraft:crimson_button";


            States["button_pressed_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
