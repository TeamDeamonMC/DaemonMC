namespace DaemonMC.Blocks
{
    public class HardOrangeStainedGlassPane : Block
    {
        public HardOrangeStainedGlassPane()
        {
            Name = "minecraft:hard_orange_stained_glass_pane";

            BlastResistance = 10;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 10;
            Opacity = 0;

            States["minecraft:connection_east"] = (byte)0;
            States["minecraft:connection_north"] = (byte)0;
            States["minecraft:connection_south"] = (byte)0;
            States["minecraft:connection_west"] = (byte)0;
        }
    }
}
