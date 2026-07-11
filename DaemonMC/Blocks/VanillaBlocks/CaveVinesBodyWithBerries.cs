namespace DaemonMC.Blocks
{
    public class CaveVinesBodyWithBerries : Block
    {
        public CaveVinesBodyWithBerries()
        {
            Name = "minecraft:cave_vines_body_with_berries";

            BlastResistance = 0;
            Brightness = 14;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["growing_plant_age"] = 0;
        }
    }
}
