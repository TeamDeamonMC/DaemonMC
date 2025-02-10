namespace DaemonMC.Blocks
{
    public class SmallDripleafBlock : Block
    {
        public SmallDripleafBlock()
        {
            Name = "minecraft:small_dripleaf_block";

            States["minecraft:cardinal_direction"] = "south";
            States["upper_block_bit"] = (byte)0;
        }
    }
}
