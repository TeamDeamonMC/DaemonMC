namespace DaemonMC.Blocks
{
    public class Bedrock : Block
    {
        public Bedrock()
        {
            Name = "minecraft:bedrock";

            BlastResistance = 3600000;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = -1;
            Opacity = 1;

            States["infiniburn_bit"] = (byte)0;
        }
    }
}
