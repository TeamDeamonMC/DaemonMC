namespace DaemonMC.Blocks
{
    public class ChainCommandBlock : Block
    {
        public ChainCommandBlock()
        {
            Name = "minecraft:chain_command_block";


            States["conditional_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
