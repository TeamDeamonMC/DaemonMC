namespace DaemonMC.Blocks
{
    public class ExposedCopperBars : Block
    {
        public ExposedCopperBars()
        {
            Name = "minecraft:exposed_copper_bars";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 5;
            Opacity = 0.19999998807907104;

            States["minecraft:connection_east"] = (byte)0;
            States["minecraft:connection_north"] = (byte)0;
            States["minecraft:connection_south"] = (byte)0;
            States["minecraft:connection_west"] = (byte)0;
        }
    }
}
