namespace DaemonMC.Blocks
{
    public class MudBrickWall : Block
    {
        public MudBrickWall()
        {
            Name = "minecraft:mud_brick_wall";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 0.19999998807907104;

            States["wall_connection_type_east"] = "none";
            States["wall_connection_type_north"] = "none";
            States["wall_connection_type_south"] = "none";
            States["wall_connection_type_west"] = "none";
            States["wall_post_bit"] = (byte)0;
        }
    }
}
