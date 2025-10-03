namespace DaemonMC.Blocks
{
    public class WaxedLightningRod : Block
    {
        public WaxedLightningRod()
        {
            Name = "minecraft:waxed_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
