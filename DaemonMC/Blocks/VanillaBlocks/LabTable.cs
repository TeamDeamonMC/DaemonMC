namespace DaemonMC.Blocks
{
    public class LabTable : Block
    {
        public LabTable()
        {
            Name = "minecraft:lab_table";

            BlastResistance = 2.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2.5;
            Opacity = 1;

            States["direction"] = 0;
        }
    }
}
