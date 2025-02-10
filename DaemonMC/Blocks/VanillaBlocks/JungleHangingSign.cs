namespace DaemonMC.Blocks
{
    public class JungleHangingSign : Block
    {
        public JungleHangingSign()
        {
            Name = "minecraft:jungle_hanging_sign";


            States["attached_bit"] = (byte)0;
            States["facing_direction"] = 0;
            States["ground_sign_direction"] = 0;
            States["hanging"] = (byte)0;
        }
    }
}
