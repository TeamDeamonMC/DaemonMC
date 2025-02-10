namespace DaemonMC.Blocks
{
    public class BambooHangingSign : Block
    {
        public BambooHangingSign()
        {
            Name = "minecraft:bamboo_hanging_sign";


            States["attached_bit"] = (byte)0;
            States["facing_direction"] = 0;
            States["ground_sign_direction"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
