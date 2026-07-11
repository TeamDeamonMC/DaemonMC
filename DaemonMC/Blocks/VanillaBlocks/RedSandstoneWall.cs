namespace DaemonMC.Blocks
{
    public class RedSandstoneWall : Block
    {
        public RedSandstoneWall()
        {
            Name = "minecraft:red_sandstone_wall";

            BlastResistance = 0.800000011920929;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.800000011920929;
            Opacity = 0.19999998807907104;

            States["wall_connection_type_east"] = "none";
            States["wall_connection_type_north"] = "none";
            States["wall_connection_type_south"] = "none";
            States["wall_connection_type_west"] = "none";
            States["wall_post_bit"] = (byte)0;
        }
    }
}
