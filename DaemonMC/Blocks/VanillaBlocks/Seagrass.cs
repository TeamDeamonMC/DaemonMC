namespace DaemonMC.Blocks
{
    public class Seagrass : Block
    {
        public Seagrass()
        {
            Name = "minecraft:seagrass";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["sea_grass_type"] = "default";
        }
    }
}
