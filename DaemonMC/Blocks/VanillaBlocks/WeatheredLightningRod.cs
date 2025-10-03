namespace DaemonMC.Blocks
{
    public class WeatheredLightningRod : Block
    {
        public WeatheredLightningRod()
        {
            Name = "minecraft:weathered_lightning_rod";

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}
