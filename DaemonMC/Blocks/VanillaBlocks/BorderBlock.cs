namespace DaemonMC.Blocks
{
    public class BorderBlock : Block
    {
        public BorderBlock()
        {
            Name = "minecraft:border_block";

            BlastResistance = 3600;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0.19999998807907104;

            States["wall_connection_type_east"] = "none";
            States["wall_connection_type_north"] = "none";
            States["wall_connection_type_south"] = "none";
            States["wall_connection_type_west"] = "none";
            States["wall_post_bit"] = (byte)0;
        }
    }
}
