namespace DaemonMC.Blocks
{
    public class CherryHangingSign : Block
    {
        public CherryHangingSign()
        {
            Name = "minecraft:cherry_hanging_sign";

            States["attached_bit"] = (byte)0;
            States["facing_direction"] = 0;
            States["ground_sign_direction"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
