namespace DaemonMC.Blocks
{
    public class SandstoneWall : Block
    {
        public SandstoneWall()
        {
            Name = "minecraft:sandstone_wall";


            States["wall_connection_type_east"] = "none";
            States["wall_connection_type_north"] = "none";
            States["wall_connection_type_south"] = "none";
            States["wall_connection_type_west"] = "none";
            States["wall_post_bit"] = (byte)0;
        }
    }
}
