namespace DaemonMC.Blocks
{
    public class WeatheredCopperBulb : Block
    {
        public WeatheredCopperBulb()
        {
            Name = "minecraft:weathered_copper_bulb";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 1;

            States["lit"] = (byte)0;
            States["powered_bit"] = (byte)0;
        }
    }
}
