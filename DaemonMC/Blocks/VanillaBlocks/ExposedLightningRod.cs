namespace DaemonMC.Blocks
{
    public class ExposedLightningRod : Block
    {
        public ExposedLightningRod()
        {
            Name = "minecraft:exposed_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
