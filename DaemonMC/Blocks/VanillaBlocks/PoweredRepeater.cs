namespace DaemonMC.Blocks
{
    public class PoweredRepeater : Block
    {
        public PoweredRepeater()
        {
            Name = "minecraft:powered_repeater";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["minecraft:cardinal_direction"] = "south";
            States["repeater_delay"] = 0;
        }
    }
}
