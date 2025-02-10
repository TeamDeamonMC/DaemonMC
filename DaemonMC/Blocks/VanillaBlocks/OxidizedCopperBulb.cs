namespace DaemonMC.Blocks
{
    public class OxidizedCopperBulb : Block
    {
        public OxidizedCopperBulb()
        {
            Name = "minecraft:oxidized_copper_bulb";

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
