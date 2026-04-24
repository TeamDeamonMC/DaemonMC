namespace DaemonMC.Blocks
{
    public class CaveVines : Block
    {
        public CaveVines()
        {
            Name = "minecraft:cave_vines";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["growing_plant_age"] = 0;
        }
    }
}
