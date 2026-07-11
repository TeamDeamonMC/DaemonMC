namespace DaemonMC.Blocks
{
    public class PaleMossCarpet : Block
    {
        public PaleMossCarpet()
        {
            Name = "minecraft:pale_moss_carpet";

            BlastResistance = 0.10000000149011612;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0.10000000149011612;
            Opacity = 0.11000001430511475;

            States["pale_moss_carpet_side_east"] = "none";
            States["pale_moss_carpet_side_north"] = "none";
            States["pale_moss_carpet_side_south"] = "none";
            States["pale_moss_carpet_side_west"] = "none";
            States["upper_block_bit"] = (byte)0;
        }
    }
}
