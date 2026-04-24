namespace DaemonMC.Blocks
{
    public class SoulCampfire : Block
    {
        public SoulCampfire()
        {
            Name = "minecraft:soul_campfire";

            BlastResistance = 2;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 0.19999998807907104;

            States["extinguished"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
