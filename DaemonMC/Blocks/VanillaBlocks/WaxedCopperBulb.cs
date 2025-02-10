namespace DaemonMC.Blocks
{
    public class WaxedCopperBulb : Block
    {
        public WaxedCopperBulb()
        {
            Name = "minecraft:waxed_copper_bulb";

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
