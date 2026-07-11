namespace DaemonMC.Blocks
{
    public class WaxedOxidizedCopperChest : Block
    {
        public WaxedOxidizedCopperChest()
        {
            Name = "minecraft:waxed_oxidized_copper_chest";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 0.5;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
