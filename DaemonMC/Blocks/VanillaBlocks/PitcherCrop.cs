namespace DaemonMC.Blocks
{
    public class PitcherCrop : Block
    {
        public PitcherCrop()
        {
            Name = "minecraft:pitcher_crop";


            States["growth"] = 0;
            States["upper_block_bit"] = (byte)0;
        }
    }
}
