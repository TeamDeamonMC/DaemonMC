namespace DaemonMC.Blocks
{
    public class BirchHangingSign : Block
    {
        public BirchHangingSign()
        {
            Name = "minecraft:birch_hanging_sign";


            States["attached_bit"] = (byte)0;
            States["facing_direction"] = 0;
            States["ground_sign_direction"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
