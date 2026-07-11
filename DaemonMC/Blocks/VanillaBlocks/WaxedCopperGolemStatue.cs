namespace DaemonMC.Blocks
{
    public class WaxedCopperGolemStatue : Block
    {
        public WaxedCopperGolemStatue()
        {
            Name = "minecraft:waxed_copper_golem_statue";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
