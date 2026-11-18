namespace DaemonMC.Blocks
{
    public class RedStainedGlassPane : Block
    {
        public RedStainedGlassPane()
        {
            Name = "minecraft:red_stained_glass_pane";

            BlastResistance = 0.30000001192092896;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.30000001192092896;
            Opacity = 0;

            States["minecraft:connection_east"] = (byte)0;
            States["minecraft:connection_north"] = (byte)0;
            States["minecraft:connection_south"] = (byte)0;
            States["minecraft:connection_west"] = (byte)0;
        }
    }
}
