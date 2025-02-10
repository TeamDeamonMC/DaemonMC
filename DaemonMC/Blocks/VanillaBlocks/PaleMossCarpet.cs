namespace DaemonMC.Blocks
{
    public class PaleMossCarpet : Block
    {
        public PaleMossCarpet()
        {
            Name = "minecraft:pale_moss_carpet";

            States["pale_moss_carpet_side_east"] = "none";
            States["pale_moss_carpet_side_north"] = "none";
            States["pale_moss_carpet_side_south"] = "none";
            States["pale_moss_carpet_side_west"] = "none";
            States["upper_block_bit"] = (byte)0;
        }
    }
}
