namespace DaemonMC.Blocks
{
    public class WaxedOxidizedLightningRod : Block
    {
        public WaxedOxidizedLightningRod()
        {
            Name = "minecraft:waxed_oxidized_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
