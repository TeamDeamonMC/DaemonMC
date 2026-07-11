namespace DaemonMC.Blocks
{
    public class EnderChest : Block
    {
        public EnderChest()
        {
            Name = "minecraft:ender_chest";

            BlastResistance = 600;
            Brightness = 7;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 22.5;
            Opacity = 0.5;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
