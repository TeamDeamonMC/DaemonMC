namespace DaemonMC.Blocks
{
    public class WaxedExposedCopperBulb : Block
    {
        public WaxedExposedCopperBulb()
        {
            Name = "minecraft:waxed_exposed_copper_bulb";

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
