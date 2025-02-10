namespace DaemonMC.Blocks
{
    public class CommandBlock : Block
    {
        public CommandBlock()
        {
            Name = "minecraft:command_block";


            States["conditional_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
