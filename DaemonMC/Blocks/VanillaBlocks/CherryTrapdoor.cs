namespace DaemonMC.Blocks
{
    public class CherryTrapdoor : Block
    {
        public CherryTrapdoor()
        {
            Name = "minecraft:cherry_trapdoor";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 0.19999998807907104;

            States["direction"] = 0;
            States["open_bit"] = (byte)0;
            States["upside_down_bit"] = (byte)0;
        }
    }
}
