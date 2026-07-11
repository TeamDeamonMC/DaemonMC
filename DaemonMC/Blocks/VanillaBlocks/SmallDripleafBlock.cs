namespace DaemonMC.Blocks
{
    public class SmallDripleafBlock : Block
    {
        public SmallDripleafBlock()
        {
            Name = "minecraft:small_dripleaf_block";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["minecraft:cardinal_direction"] = "south";
            States["upper_block_bit"] = (byte)0;
        }
    }
}
