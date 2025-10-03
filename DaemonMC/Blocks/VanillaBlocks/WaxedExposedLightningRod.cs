namespace DaemonMC.Blocks
{
    public class WaxedExposedLightningRod : Block
    {
        public WaxedExposedLightningRod()
        {
            Name = "minecraft:waxed_exposed_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
