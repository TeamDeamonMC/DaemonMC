namespace DaemonMC.Blocks
{
    public class BambooSapling : Block
    {
        public BambooSapling()
        {
            Name = "minecraft:bamboo_sapling";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0;

            States["age_bit"] = (byte)0;
        }
    }
}
