namespace DaemonMC.Blocks
{
    public class DarkOakSapling : Block
    {
        public DarkOakSapling()
        {
            Name = "minecraft:dark_oak_sapling";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["age_bit"] = (byte)0;
        }
    }
}
