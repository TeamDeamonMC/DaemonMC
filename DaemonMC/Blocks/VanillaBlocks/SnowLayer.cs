namespace DaemonMC.Blocks
{
    public class SnowLayer : Block
    {
        public SnowLayer()
        {
            Name = "minecraft:snow_layer";

            BlastResistance = 0.10000000149011612;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.10000000149011612;
            Opacity = 0.11000001430511475;

            States["covered_bit"] = (byte)0;
            States["height"] = 0;
        }
    }
}
