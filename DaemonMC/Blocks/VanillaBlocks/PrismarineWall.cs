namespace DaemonMC.Blocks
{
    public class PrismarineWall : Block
    {
        public PrismarineWall()
        {
            Name = "minecraft:prismarine_wall";


            States["wall_connection_type_east"] = "none";
            States["wall_connection_type_north"] = "none";
            States["wall_connection_type_south"] = "none";
            States["wall_connection_type_west"] = "none";
            States["wall_post_bit"] = (byte)0;
        }
    }
}
