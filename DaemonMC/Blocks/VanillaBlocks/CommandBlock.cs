namespace DaemonMC.Blocks
{
    public class CommandBlock : Block
    {
        public CommandBlock()
        {
            Name = "minecraft:command_block";

            BlastResistance = 3600000;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = -1;
            Opacity = 1;

            States["conditional_bit"] = (byte)0;
            States["facing_direction"] = 0;
        }
    }
}
