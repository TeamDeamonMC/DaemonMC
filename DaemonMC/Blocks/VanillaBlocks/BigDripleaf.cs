namespace DaemonMC.Blocks
{
    public class BigDripleaf : Block
    {
        public BigDripleaf()
        {
            Name = "minecraft:big_dripleaf";

            BlastResistance = 0.10000000149011612;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0.10000000149011612;
            Opacity = 0;

            States["big_dripleaf_head"] = (byte)0;
            States["big_dripleaf_tilt"] = "none";
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}
