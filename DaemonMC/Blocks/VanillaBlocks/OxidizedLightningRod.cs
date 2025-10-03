namespace DaemonMC.Blocks
{
    public class OxidizedLightningRod : Block
    {
        public OxidizedLightningRod()
        {
            Name = "minecraft:oxidized_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
