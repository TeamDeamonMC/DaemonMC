namespace DaemonMC.Blocks
{
    public class WeatheredCopperBulb : Block
    {
        public WeatheredCopperBulb()
        {
            Name = "minecraft:weathered_copper_bulb";

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
