namespace DaemonMC.Blocks
{
    public class PitcherCrop : Block
    {
        public PitcherCrop()
        {
            Name = "minecraft:pitcher_crop";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["growth"] = 0;
            States["upper_block_bit"] = (byte)0;
        }
    }
}
