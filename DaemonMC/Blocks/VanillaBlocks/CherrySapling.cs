namespace DaemonMC.Blocks
{
    public class CherrySapling : Block
    {
        public CherrySapling()
        {
            Name = "minecraft:cherry_sapling";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["age_bit"] = (byte)0;
        }
    }
}
