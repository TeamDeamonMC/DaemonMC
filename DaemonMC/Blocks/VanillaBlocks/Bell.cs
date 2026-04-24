namespace DaemonMC.Blocks
{
    public class Bell : Block
    {
        public Bell()
        {
            Name = "minecraft:bell";

            BlastResistance = 5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 5;
            Opacity = 1;

            States["attachment"] = "standing";
            States["direction"] = 0;
            States["toggle_bit"] = (byte)0;
        }
    }
}
