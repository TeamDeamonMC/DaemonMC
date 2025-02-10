namespace DaemonMC.Blocks
{
    public class ExposedCopperBulb : Block
    {
        public ExposedCopperBulb()
        {
            Name = "minecraft:exposed_copper_bulb";


            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
