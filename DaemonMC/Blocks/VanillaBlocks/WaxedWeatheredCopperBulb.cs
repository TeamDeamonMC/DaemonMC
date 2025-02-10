namespace DaemonMC.Blocks
{
    public class WaxedWeatheredCopperBulb : Block
    {
        public WaxedWeatheredCopperBulb()
        {
            Name = "minecraft:waxed_weathered_copper_bulb";

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
