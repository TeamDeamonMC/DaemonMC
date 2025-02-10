namespace DaemonMC.Blocks
{
    public class WaxedOxidizedCopperBulb : Block
    {
        public WaxedOxidizedCopperBulb()
        {
            Name = "minecraft:waxed_oxidized_copper_bulb";


            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
