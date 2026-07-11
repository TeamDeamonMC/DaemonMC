namespace DaemonMC.Blocks
{
    public class TallGrass : Block
    {
        public TallGrass()
        {
            Name = "minecraft:tall_grass";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 60;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["upper_block_bit"] = (byte)0;
        }
    }
}
