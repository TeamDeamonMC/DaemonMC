namespace DaemonMC.Blocks
{
    public class DarkOakLeaves : Block
    {
        public DarkOakLeaves()
        {
            Name = "minecraft:dark_oak_leaves";

            BlastResistance = 0.20000000298023224;
            Brightness = 0;
            FlameEncouragement = 30;
            Flammability = 60;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0.5;

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}
