namespace DaemonMC.Blocks
{
    public class RepeatingCommandBlock : Block
    {
        public RepeatingCommandBlock()
        {
            Name = "minecraft:repeating_command_block";

            States["conditional_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
