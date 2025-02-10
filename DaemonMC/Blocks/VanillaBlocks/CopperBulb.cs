namespace DaemonMC.Blocks
{
    public class CopperBulb : Block
    {
        public CopperBulb()
        {
            Name = "minecraft:copper_bulb";

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
