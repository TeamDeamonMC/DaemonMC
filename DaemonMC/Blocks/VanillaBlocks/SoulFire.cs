namespace DaemonMC.Blocks
{
    public class SoulFire : Block
    {
        public SoulFire()
        {
            Name = "minecraft:soul_fire";

            BlastResistance = 0;
            Brightness = 10;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["age"] = 0;
        }
    }
}
