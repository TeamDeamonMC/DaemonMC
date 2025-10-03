namespace DaemonMC.Blocks
{
    public class WaxedWeatheredLightningRod : Block
    {
        public WaxedWeatheredLightningRod()
        {
            Name = "minecraft:waxed_weathered_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
