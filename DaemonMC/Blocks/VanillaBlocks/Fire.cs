namespace DaemonMC.Blocks
{
    public class Fire : Block
    {
        public Fire()
        {
            Name = "minecraft:fire";

            BlastResistance = 0;
            Brightness = 15;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["age"] = 0;
            States["minecraft:connection_east"] = (byte)0;
            States["minecraft:connection_north"] = (byte)0;
            States["minecraft:connection_south"] = (byte)0;
            States["minecraft:connection_up"] = (byte)0;
            States["minecraft:connection_west"] = (byte)0;
        }
    }
}
