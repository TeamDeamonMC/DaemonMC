namespace DaemonMC.Blocks
{
    public class StrawBed : Block
    {
        public StrawBed()
        {
            Name = "minecraft:straw_bed";

            BlastResistance = 0.20000000298023224;
            Brightness = 0;
            FlameEncouragement = 30;
            Flammability = 60;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 1;

            States["head_piece_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["occupied_bit"] = (byte)0;
        }
    }
}
